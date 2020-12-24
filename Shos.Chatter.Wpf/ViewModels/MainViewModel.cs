using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Shos.Chatter.Wpf.ViewModels
{
    using Models;
    using Shos.Chatter.Server.Models;

    public class MainViewModel : BindableBase
    {
        class CommandBase : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            protected ChatterModel Model { get; private set; }

            public CommandBase(ChatterModel model) => Model = model;

            public virtual bool CanExecute(object? parameter) => true;

            public virtual void Execute(object? parameter)
            {}
        }

        class AddUserCommandType : CommandBase
        {
            public AddUserCommandType(ChatterModel model) : base(model)
            {}

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;
                var name = (string)parameter;
                if (!string.IsNullOrWhiteSpace(name))
                    Model.Add(new User { Name = name });
            }
        }

        class UpdateUserCommandType : CommandBase
        {
            public UpdateUserCommandType(ChatterModel model) : base(model)
            { }

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;
                var id = (int)parameter;
                var user = Model.Users.FirstOrDefault(u => u.Id == id);
                if (user is not null)
                    Model.Update(user);
            }
        }

        class DeleteUserCommandType : CommandBase
        {
            public DeleteUserCommandType(ChatterModel model) : base(model)
            {}

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;

                var id = (int)parameter;
                var user = Model.Users.FirstOrDefault(u => u.Id == id);
                if (user is not null)
                    Model.Delete(user);
            }
        }

        class AddChatCommandType : CommandBase
        {
            public AddChatCommandType(ChatterModel model) : base(model)
            { }

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;

                var message = (string)parameter;
                if (!string.IsNullOrWhiteSpace(message))
                    Model.Add(new Chat { Message = message });
            }
        }

        class UpdateChatCommandType : CommandBase
        {
            public UpdateChatCommandType(ChatterModel model) : base(model)
            { }

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;

                var id = (int)parameter;
                var chat = Model.Chats.FirstOrDefault(c => c.Id == id);
                if (chat is not null)
                    Model.Update(chat);
            }
        }

        class DeleteChatCommandType : CommandBase
        {
            public DeleteChatCommandType(ChatterModel model) : base(model)
            { }

            public override void Execute(object? parameter)
            {
                if (parameter is null)
                    return;

                var id = (int)parameter;
                var chat = Model.Chats.FirstOrDefault(c => c.Id == id);
                if (chat is not null)
                    Model.Delete(chat);
            }
        }

        ChatterModel model = new ChatterModel();

        public ICommand AddUserCommand    { get; private set; }
        public ICommand UpdateUserCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }

        public ICommand AddChatCommand { get; private set; }
        public ICommand UpdateChatCommand { get; private set; }
        public ICommand DeleteChatCommand { get; private set; }

        public MainViewModel()
        {
            model.PropertyChanged += (_, e) => RaisePropertyChanged(e.PropertyName);

            AddUserCommand    = new AddUserCommandType   (model);
            UpdateUserCommand = new UpdateUserCommandType(model);
            DeleteUserCommand = new DeleteUserCommandType(model);

            AddChatCommand    = new AddChatCommandType   (model);
            UpdateChatCommand = new UpdateChatCommandType(model);
            DeleteChatCommand = new DeleteChatCommandType(model);

            model.Start();
        }

        public int UserId {
            get => model.UserId;
            set => model.UserId = value;
        }

        public IEnumerable<User> Users => model.Users;
        public IEnumerable<Chat> Chats => model.Chats;
    }
}
