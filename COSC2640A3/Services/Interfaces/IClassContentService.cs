﻿using System.Threading.Tasks;
using COSC2640A3.Models;
using COSC2640A3.ViewModels.ClassContent;

namespace COSC2640A3.Services.Interfaces {

    public interface IClassContentService {

        //Task<bool?> InsertOrSuplementFilesContentByClassroomId(string classroomId, FileVM[] uploadResults);
        
        Task<ClassContent> GetClassContentByClassroomId(string classroomId);
        
        Task<bool?> UpdateContent(ClassContent classContent);
        
        Task<string> InsertNewContent(ClassContent classContent);
    }
}