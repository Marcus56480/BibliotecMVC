using System.Security.Claims;
using Azure.Identity;
using BibliotecMVC.Data;
using BibliotecMVC.Models;
using BibliotecMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecMVC.Controllers;

public class LoginController : Controller
{

    //Recebendo Dependencias (Registradas na program)
    // Permite consultar e manipular os dados do banco
    private readonly AppDbContext _context;
    // Permite gerar e validar o hash das senhas
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public LoginController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // Mostra a visualização da tela de login
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // Recebe os dados enviado pelo usuario
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModels model)
    {
        // Verifica se os dados recebidos atendem as regras de validação 

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Mensagem temporaria para testar
        // ModelState.AddModelError("", "Autenticação ainda não implementada");

        // Busca um usuario pelo email informado e verifica se ele não foi excluido;
        // se não encontrar retorna null
        var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == model.Email && u.Excluido == false);

        // Verifica se o usuario não foi encontrado;
        if (usuario == null)
        {
            ModelState.AddModelError("", "Email ou Senha Invalidos.");
            return View(model);
        }

        // Verifica se a senha informada corresponde ao hash informado;
        // O metodo recebe 3 parametros;

        var resultadoSenha = _passwordHasher.VerifyHashedPassword(
            usuario, // O objeto que representa o usuario encontrado no banco;
            usuario.Senha, // Hash da senha armazenada no banco;
            model.Senha // Senha digitada pelo usuário no formulário de login;
        );

        // Verifica se a senha está Incorreta;
        if (resultadoSenha == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Email ou Senha Invalidos.");
            return View(model);
        }

        // Verifica se a senha está correta, mas o hash precisa ser atualizado;
        if (resultadoSenha == PasswordVerificationResult.Success) ;
        {
            // Gera um novo hash da senha utilizando as configurações de segurança atuais;
            usuario.Senha = _passwordHasher.HashPassword(usuario, model.Senha);

            // Salva o novo hash no banco de dados;
            await _context.SaveChangesAsync();
        }

        // Verifica se o usuario está cadastrado como usuario ou administrador;
        // Verifica dentro da tabela ADM se o usuario possui registro;
        // Retorna true se existir pelo menos um registro que atende a condição informada;
        bool administrador = await _context.Administrador.AnyAsync(a => a.IdAdministrador == usuario.IdUsuario);

        // Verifica se o usuario está cadastrado como aluno
        bool aluno = await _context.Aluno.AnyAsync(a => a.IdAluno == usuario.IdUsuario);

        // Coloca o tipo de perfil de usuario na variavel tipoUsuario
        string? tipoUsuario = administrador ? "Administrador" : aluno ? "Aluno" : null;

        // Cria uma lista de claim com as informações do usuario autenticado;
        // Claim -> Informações do usuário, que permitem identificar e controlar o acesso.
        var claims = new List<Claim>
        {
            // Armazena o identificador do usuario (id);
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            // Armazena o nome do usuario;
            new Claim(ClaimTypes.Name, usuario.Nome),
            // Armazena o emial do usuario;
            new Claim(ClaimTypes.Email, usuario.Email),
            // Armazena o tipo de usuario para controle de acesso;
            new Claim(ClaimTypes.Role, tipoUsuario)
        };
        // Cria a identidade do usuario criando as claims;
        // -> ClaimsIdentity -> Como se fosse uma carteirinha de identificação para o usuario dentro do sistema;
        // Cookies -> Informa que estamos utilizando o tipo de autenticação por cookies;
        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        // Representa o usuario autenticado;
        // Classe do asp.net para representar o usuário;
        var principal = new ClaimsPrincipal(identidade);
        
        // Realiza a autenticação e gera o cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        // Redireciona o usuario para a tela Home, após o login ser bem-sucessidido
        return RedirectToAction("Index", "Home");
    }
}