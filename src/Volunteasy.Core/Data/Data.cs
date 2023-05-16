using Microsoft.EntityFrameworkCore;

namespace Volunteasy.Core.Data;

public class Data : DbContext
{
    public Data(DbContextOptions opt) : base(opt) {}
}