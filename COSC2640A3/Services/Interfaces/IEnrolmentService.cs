using System.Threading.Tasks;
using COSC2640A3.Models;
using COSC2640A3.ViewModels.Exports;
using COSC2640A3.ViewModels.Features;

namespace COSC2640A3.Services.Interfaces {

    public interface IEnrolmentService {

        /// <summary>
        /// Returns default if error, otherwise returns the ID of new enrolment.
        /// </summary>
        Task<string> InsertNewEnrolment(Enrolment enrolment);

        /// <summary>
        /// Returns default if error, otherwise returns the ID of new invoice.
        /// </summary>
        Task<string> InsertNewInvoice(Invoice invoice);

        /// <summary>
        /// Returns null if error, returns false if not belonged, returns true if belonged.
        /// </summary>
        Task<bool?> IsEnrolmentMadeByStudentByAccountId(string enrolmentId, string accountId);

        Task<Enrolment> GetEnrolmentById(string enrolmentId);
        
        /// <summary>
        /// Returns null if error, returns false if failed, returns true if success.
        /// </summary>
        Task<bool?> DeleteEnrolment(Enrolment enrolment);

        Task<Invoice> GetInvoiceByEnrolmentId(string enrolmentId);
        
        /// <summary>
        /// Returns null if error, returns false if failed, returns true if success.
        /// </summary>
        Task<bool?> DeleteInvoice(Invoice invoice);

        Task<EnrolmentVM[]> GetStudentEnrolmentsByAccountId(string accountId);
        
        /// <summary>
        /// Returns null if error, returns false if failed, returns true if success.
        /// </summary>
        Task<bool?> UpdateEnrolment(Enrolment enrolment);

        /// <summary>
        /// Returns null if error, returns false if not associated, returns true if associated.
        /// </summary>
        Task<bool?> DoesEnrolmentRelateToAClassroomOfThisTeacher(string enrolmentId, string teacherId);

        Task<bool?> UpdateMultipleEnrolments(Enrolment[] enrolments);
        
        EnrolmentExportVM[] GetEnrolmentDataForExportBy(string[] classroomIds);
    }
}