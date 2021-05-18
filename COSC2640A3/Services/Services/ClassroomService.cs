﻿using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels.Exports;
using COSC2640A3.ViewModels.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class ClassroomService : IClassroomService {
        
        private readonly ILogger<ClassroomService> _logger;
        private readonly MainDbContext _dbContext;

        public ClassroomService(
            ILogger<ClassroomService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> InsertNewClassroom(Classroom classroom) {
            await _dbContext.Classrooms.AddAsync(classroom);

            try {
                await _dbContext.SaveChangesAsync();
                return classroom.Id;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(InsertNewClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateClassroom(Classroom classroom) {
            _dbContext.Classrooms.Update(classroom);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(UpdateClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> IsClassroomBelongedToThisTeacherByAccountId(string accountId, string classroomId) {
            try {
                return await _dbContext.Teachers
                                       .Where(teacher => teacher.AccountId.Equals(accountId))
                                       .SelectMany(teacher => teacher.Classrooms)
                                       .AnyAsync(classroom => classroom.Id.Equals(classroomId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(IsClassroomBelongedToThisTeacherByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Classroom> GetClassroomById(string classroomId) {
            return await _dbContext.Classrooms.FindAsync(classroomId);
        }

        public async Task<bool?> DeleteClassroom(Classroom classroom) {
            _dbContext.Classrooms.Remove(classroom);
            
            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(ClassroomService) }.{ nameof(DeleteClassroom) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<ClassroomVM[]> GetAllClassroomsByTeacherId(string teacherId, bool isActive = true) {
            try {
                var classroomData = await _dbContext.Classrooms
                                       .Where(classroom => classroom.TeacherId.Equals(teacherId) && classroom.IsActive == isActive)
                                       .Select(classroom => new { classroom = (ClassroomVM) classroom, enrolments = classroom.Enrolments.Count })
                                       .ToArrayAsync();

                return classroomData.Select(data => {
                                        var classroom = data.classroom;
                                        classroom.EnrolmentsCount = data.enrolments;
                                        return classroom;
                                    })
                                    .ToArray();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(GetAllClassroomsByTeacherId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<ClassroomVM[]> GetAllClassroomsExcludeFromTeacherId(string teacherId) {
            try {
                return await _dbContext.Classrooms
                                       //.Where(classroom => !classroom.TeacherId.Equals(teacherId))
                                       .Select(classroom => new ClassroomVM {
                                           Id = classroom.Id,
                                           TeacherId = classroom.TeacherId,
                                           TeacherName = classroom.Teacher.Account.PreferredName,
                                           ClassName = classroom.ClassName,
                                           Price = classroom.Price
                                       })
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(ClassroomService) }.{ nameof(GetAllClassroomsExcludeFromTeacherId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<ClassroomVM> GetClassroomDetailsFor(string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomService) }.{ nameof(GetClassroomDetailsFor) }: { nameof(classroomId) }={ classroomId }");

            var dbClassroom = await _dbContext.Classrooms
                                              .Where(classroom => classroom.Id.Equals(classroomId))
                                              .Select(classroom => new { Classroom = classroom, TeacherName = classroom.Teacher.Account.PreferredName })
                                              .SingleAsync();

            var classroomDetail = new ClassroomVM {
                Id = dbClassroom.Classroom.Id,
                TeacherId = dbClassroom.Classroom.TeacherId,
                TeacherName = dbClassroom.TeacherName,
                ClassName = dbClassroom.Classroom.ClassName,
                Price = dbClassroom.Classroom.Price
            };
            
            classroomDetail.SetClassroomDetail(dbClassroom.Classroom);
                
            return classroomDetail;
        }

        public async Task<EnrolmentVM[]> GetStudentEnrolmentsByClassroomId(string classroomId) {
            try {
                var enrolmentData = await _dbContext.Enrolments
                                                    .Where(enrolment => enrolment.ClassroomId.Equals(classroomId))
                                                    .Select(enrolment => new { envolmentVm = (EnrolmentVM) enrolment, invoice = enrolment.Invoice })
                                                    .ToArrayAsync();

                return enrolmentData
                       .Select(pair => {
                           var enrolment = pair.envolmentVm;
                           if (pair.invoice.IsPaid) enrolment.Invoice.PaymentDetail = pair.invoice;
                           return enrolment;
                       })
                       .ToArray();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetStudentEnrolmentsByClassroomId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<Enrolment[]> GetEnrolmentsByClassroomId(string classroomId) {
            try {
                return await _dbContext.Enrolments
                                       .Where(enrolment => enrolment.ClassroomId.Equals(classroomId))
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetEnrolmentsByClassroomId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<bool?> AreTheseClassroomsBelongedTo(string accountId) {
            try {
                return await _dbContext.Classrooms.AllAsync(classroom => classroom.Teacher.AccountId.Equals(accountId));
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(AreTheseClassroomsBelongedTo) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<ClassroomExportVM[]> GetClassroomDataForExportBy(string[] classroomIds) {
            try {
                return await _dbContext.Classrooms
                                       .Where(classroom => classroomIds.Contains(classroom.Id))
                                       .Select(classroom => (ClassroomExportVM) classroom)
                                       .ToArrayAsync();
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(EnrolmentService) }.{ nameof(GetClassroomDataForExportBy) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }
    }
}