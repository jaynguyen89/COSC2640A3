using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Helper.Shared.SharedConstants;
using static Helper.Shared.SharedEnums;

namespace AssistantLibrary.Services {

    public sealed class DictionaryService : IDictionaryService {

        private readonly ILogger<DictionaryService> _logger;
        private readonly HttpClient _httpClient = new();

        private const string AppIdHeader = "app_id";
        private const string AppKeyHeader = "app_key";

        public DictionaryService(
            ILogger<DictionaryService> logger,
            IOptions<AssistantOptions> options
        ) {
            _logger = logger;
            
            _httpClient.BaseAddress = new Uri(options.Value.OxfordDictionaryApiEndpoint);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add(AppIdHeader, options.Value.OxfordDictionaryAppApiId);
            _httpClient.DefaultRequestHeaders.Add(AppKeyHeader, options.Value.OxfordDictionaryAppApiKey);
        }

        public async Task<Translation> GetSynonymsOrAntonymsFor(string word, bool synonym = true) {
            _logger.LogInformation($"private { nameof(DictionaryService) }.{ nameof(GetSynonymsOrAntonymsFor) }: Service starts.");
            
            var rootWord = await GetRootWord(word);
            if (rootWord is null) return default;
            
            var urlQuery = $"thesaurus/{ LanguageCodes[(byte) Language.English] }/{ word }?fields={ (synonym ? "synonyms" : "antonyms") }&strictMatch=false";
            
            var response = await _httpClient.GetAsync(urlQuery, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return default;

            var thesaurusResponse = JsonConvert.DeserializeObject<ThesaurusParser>(await response.Content.ReadAsStringAsync());
            if (thesaurusResponse is null) return default;

            var thesaurus = new Translation {
                Word = word,
                RootForm = rootWord,
                WordTypes = thesaurusResponse.ThesaurusEntries
                                             .SelectMany(thesaurusEntry =>
                                                 thesaurusEntry.LexicalEntries
                                                               .Select(lexicon => lexicon.WordType.Id)
                                                               .ToArray()
                                             )
                                             .Distinct()
                                             .ToArray(),
                Synonyms = new Dictionary<string, string[]>(),
                Antonyms = new Dictionary<string, string[]>()
            };
            
            foreach (var type in thesaurus.WordTypes)
                if (synonym) {
                    var wordsByType =
                        thesaurusResponse.ThesaurusEntries
                                         .SelectMany(thesaurusEntry =>
                                             thesaurusEntry.LexicalEntries
                                                           .Where(lexicon => lexicon.WordType.Id.Equals(type))
                                                           .SelectMany(lexicon =>
                                                               lexicon.Entries.SelectMany(entry =>
                                                                   entry.Senses.SelectMany(sense => sense.Synonyms.Select(s => s.Text))
                                                               )
                                                           )
                                         )
                                         .ToArray();
                    
                    thesaurus.Synonyms.Add(type, wordsByType);
                }
                else {
                    var wordsByType =
                        thesaurusResponse.ThesaurusEntries
                                         .SelectMany(thesaurusEntry =>
                                             thesaurusEntry.LexicalEntries
                                                           .Where(lexicon => lexicon.WordType.Id.Equals(type))
                                                           .SelectMany(lexicon =>
                                                               lexicon.Entries.SelectMany(entry =>
                                                                   entry.Senses.SelectMany(sense => sense.Antonyms.Select(a => a.Text))
                                                               )
                                                           )
                                         )
                                         .ToArray();
                    
                    thesaurus.Antonyms.Add(type, wordsByType);
                }

            return thesaurus;
        }

        public async Task<Translation> Translate(Language target, string word) {
            _logger.LogInformation($"private { nameof(DictionaryService) }.{ nameof(Translate) }: Service starts.");
            
            var rootWord = await GetRootWord(word);
            if (rootWord is null) return default;

            var urlQuery = $"translations/{ LanguageCodes[(byte) Language.English] }/{ LanguageCodes[(byte) target] }/{ rootWord }?strictMatch=false&fields=definitions%2Cpronunciations%2Ctranslations";
            
            var response = await _httpClient.GetAsync(urlQuery, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return default;

            var translationResponse = JsonConvert.DeserializeObject<TranslationParser>(await response.Content.ReadAsStringAsync());
            if (translationResponse is null) return default;

            var translation = new Translation {
                Word = word,
                RootForm = rootWord,
                WordTypes = translationResponse.TranslationEntries
                                               .SelectMany(translationEntry =>
                                                   translationEntry.LexicalEntries
                                                                   .Select(lexicon => lexicon.WordType.Id)
                                                                   .ToArray()
                                               )
                                               .Distinct()
                                               .ToArray(),
                Translations = new Dictionary<string, string[]>()
            };
            
            foreach (var type in translation.WordTypes) {
                var wordsByType =
                    translationResponse.TranslationEntries
                                       .SelectMany(translationEntry =>
                                           translationEntry.LexicalEntries
                                                           .Where(lexicon => lexicon.WordType.Id.Equals(type))
                                                           .SelectMany(lexicon =>
                                                               lexicon.Entries.SelectMany(entry =>
                                                                   entry.Senses.SelectMany(sense => sense.Translations.Select(t => t.Text))
                                                               )
                                                           )
                                       )
                                       .ToArray();
                
                translation.Translations.Add(type, wordsByType);
            }

            return translation;
        }

        private async Task<string> GetRootWord(string word) {
            _logger.LogInformation($"private { nameof(DictionaryService) }.{ nameof(GetRootWord) }: Service starts.");

            var urlQuery = $"lemmas/{ LanguageCodes[(byte) Language.English] }/{ word }";

            var response = await _httpClient.GetAsync(urlQuery, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return default;

            var lemmaResponse = JsonConvert.DeserializeObject<LemmaParser>(await response.Content.ReadAsStringAsync());
            return lemmaResponse?.LemmaEntries[0].LexicalEntries[0].Inflection[0].Id;
        }
    }
}