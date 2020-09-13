using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetNote.Models;
using NetNote.ViewModels;

namespace NetNote.Pages
{
    public class LoginModel : PageModel
    {
        private SignInManager<NoteUser> _signInManager;
        public LoginModel(SignInManager<NoteUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginViewModel Login { get; set; }
        [BindProperty(SupportsGet =true)]
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Login.UserName, Login.Password, Login.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误");
                return Page();
            }
        }
    }
}
