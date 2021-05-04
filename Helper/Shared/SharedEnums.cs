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
            Completed
        }
        
        public enum ImportType {
            Classroom,
            Students,
            Both
        }
        
        public enum EmailType {
            AccountActivation,
            AccountActivationConfirmation,
            PasswordRecovery,
            TwoFaPin,
            TwoFaDisabledNotification
        }
    }
}
