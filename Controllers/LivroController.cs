
using System.Threading.Tasks.Dataflow;
using BibliotecMVC.Data;
using BibliotecMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecMVC.Controllers;

// Permite o acesso apenas aos usuarios autenticados (adm e aluno)
[Authorize(Roles = "Administrador, Aluno")]
public class LivroController : Controller
{
    private readonly AppDbContext _context;

    public LivroController(AppDbContext context)
    {
        _context = context;
    }

    // Lista os livros cadastrados no banco de dados
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var livros = await _context.Livro
            .Include(l => l.IdAutorNavigation)
            .Include(l => l.IdStatusNavigation)
            .Include(l => l.IdCategoria)
            .OrderBy(l => l.Titulo)
            .ToListAsync();

            if (User.IsInRole("Administrador"))
        {
            // Retorna a view de listagem de adm enviando a lista de livros
            return View("ListagemAdministrador", livros);
        }

        // retorna a view de listagem de alunos enviando a lista de livros
        return View("ListagemAluno", livros);
    }
}
