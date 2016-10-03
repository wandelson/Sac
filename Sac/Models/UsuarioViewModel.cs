using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sac.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [DisplayName("E-mail")]
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        public string Email { get; set; }

   

        [DisplayName("Usuário")]
        [Required(ErrorMessage = "O login é obrigatório")]
        public string Username { get; set; }

        [DisplayName("Senha")]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        [DataType(DataType.Password)]

        [DisplayName("Confirmação senha")]
        [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação não conferem! Digite novamente!")]
        public string Confirma { get; set; }

        [Required(ErrorMessage = "O papel é obrigatório")]
        public string Papel { get; set; }


    }
}