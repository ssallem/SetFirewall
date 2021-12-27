﻿namespace SetFirewall.Domain
{
    public class FieldsViewModel : ViewModelBase
    {
        private string? _name;
        private string? _name2;
        private string? _name3;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? Name2
        {
            get => _name2;
            set => SetProperty(ref _name2, value);
        }

        public string? Name3
        {
            get => _name3;
            set => SetProperty(ref _name3, value);
        }

        public FieldsTestObject TestObject => new() { Name = "Mr. Test" };
    }

    public class FieldsTestObject : ViewModelBase
    {
        private string? _name;
        private string? _content;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }
}
