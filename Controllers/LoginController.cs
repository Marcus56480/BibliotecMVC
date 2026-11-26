using BibliotecMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecMVC.Controllers;

public class LoginController : Controller
{   
    // Mostra a visualização da tela de login
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // Recebe os dados enviado pelo usuario
    [HttpPost]
    public IActionResult Index(LoginViewModels model)
    {
        // Verifica se os dados recebidos atendem as regras de validação 

        if(!ModelState.IsValid)
        {
            return View(model);
        }

        // Mensagem temporaria para testar

        ModelState.AddModelError("", "Autenticação ainda não implementada");

        return View(model);
    }
}