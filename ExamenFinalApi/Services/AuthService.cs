using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contratos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ExamenFinalApi.Data;
using ExamenFinalApi.Interfaces;
using ExamenFinalApi.Models;

namespace ExamenFinalApi.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly BaseDatos _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public AuthService(BaseDatos context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public string? Registrar(LoginDto dto)
        {
            var nombreUsuario = dto.NombreUsuario.Trim();

            if (_context.Usuarios.Any(u => u.NombreUsuario == nombreUsuario))
                return "Ese nombre de usuario ya existe.";

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombreUsuario,
                Rol = "Usuario"          // quien se registra solo puede VER; el admin se crea en las semillas
            };

            // Encripta la contraseña: en la base nunca se guarda en texto plano
            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return null;
        }

        public LoginRespuestaDto? Login(LoginDto dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == dto.NombreUsuario.Trim());
            if (usuario == null)
                return null;

            // Compara la contraseña escrita con la encriptada
            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);
            if (resultado == PasswordVerificationResult.Failed)
                return null;

            return new LoginRespuestaDto
            {
                Token = GenerarToken(usuario),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol
            };
        }

        // Crea el token JWT: un "pase" con el nombre y el rol que dura 2 horas
        private string GenerarToken(Usuario usuario)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Clave"]!));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
