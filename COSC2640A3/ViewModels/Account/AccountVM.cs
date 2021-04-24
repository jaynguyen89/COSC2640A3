namespace COSC2640A3.ViewModels.Account {

    public class AccountVM {

        public string Email { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFaEnabled { get; set; }

        public string PreferredName { get; set; }
        
        public TwoFaVM TwoFa { get; set; }
    }
}