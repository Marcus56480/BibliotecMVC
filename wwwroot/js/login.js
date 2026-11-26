const campoSenha = document.getElementById("senha");
const botaoMostrarSenha = document.getElementById("mostrar-senha");

// Alterna entre mostrar e ocultar a senha.
botaoMostrarSenha.addEventListener("click", (e) => {

    e.preventDefault(); //FIXME: Botão envia os dados ao invés de alternar de password para text
    
    if (campoSenha.type === "password") {
        campoSenha.type = "text";
    } else {
        campoSenha.type = "password";
    }

});