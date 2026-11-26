using System.ComponentModel.DataAnnotations;

namespace BibliotecMVC.ViewModels;

public class LoginViewModels
{
    [Required(ErrorMessage = "Informe o E-mail.")]
    [EmailAddress(ErrorMessage = "Informe um E-mail válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a Senha")]
    public string Senha { get; set; } = string.Empty;
}