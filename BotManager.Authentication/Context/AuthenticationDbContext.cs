using Microsoft.EntityFrameworkCore;

namespace BotManager.Authentication.Context;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options);