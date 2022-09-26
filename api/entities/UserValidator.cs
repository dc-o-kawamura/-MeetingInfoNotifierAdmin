using System;
using FluentValidation;

namespace MeetingInfoNotifierAdmin.Api.Entities
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            // ユーザー名が未指定の場合はNGとする。
            RuleFor(user => user.Name).NotNull().NotEmpty().WithMessage("name is null or empty");
            // メールアドレスが未指定の場合はNGとする。
            RuleFor(user => user.EmailAddress).NotNull().NotEmpty().WithMessage("emailAddress is null or empty");
        }
    }
}