namespace Helper.Shared {

    public static class SharedEnums {

        public enum RequestResult {
            Failed,
            Success
        }

        public enum Role {
            Student,
            Teacher
        }

        public enum DurationUnit {
            Hours,
            Days,
            Weeks,
            Months,
            Years
        }

        public enum ScheduleStatus {
            Awaiting,
            Processing,
            Completed,
            Partial,
            Failed
        }
        
        public enum ImportType {
            Classroom,
            Students
        }
        
        public enum EmailType {
            AccountActivationConfirmation,
            PasswordRecovery,
            TwoFaPin,
            TwoFaDisabledNotification
        }

        public enum NotificationType {
            Email,
            Sms,
            Both
        }

        public enum FileType {
            video,
            audio,
            photo,
            other // must always be at bottom
        }
    }
}
