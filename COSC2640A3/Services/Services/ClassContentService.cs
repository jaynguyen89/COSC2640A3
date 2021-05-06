﻿using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class ClassContentService : IClassContentService {
        
        private readonly ILogger<ClassContentService> _logger;
        private readonly MainDbContext _dbContext;

        public ClassContentService(
            ILogger<ClassContentService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ClassContent> GetClassContentByClassroomId(string classroomId) {
            try {
                return _dbContext.ClassContents.SingleOrDefault(content => content.ClassroomId.Equals(classroomId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassContentService) }.{ nameof(GetClassContentByClassroomId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidOperationException e) {
                _logger.LogWarning($"{ nameof(ClassContentService) }.{ nameof(GetClassContentByClassroomId) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateContent(ClassContent classContent) {
            _dbContext.ClassContents.Update(classContent);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassContentService) }.{ nameof(UpdateContent) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string> InsertNewContent(ClassContent classContent) {
            await _dbContext.ClassContents.AddAsync(classContent);

            try {
                await _dbContext.SaveChangesAsync();
                return classContent.Id;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassContentService) }.{ nameof(InsertNewContent) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}