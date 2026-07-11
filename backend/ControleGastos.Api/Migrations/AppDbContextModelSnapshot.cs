using ControleGastos.Api.Data; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Infrastructure;
namespace ControleGastos.Api.Migrations;
[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
 protected override void BuildModel(ModelBuilder b)
 {
  b.HasAnnotation("ProductVersion","8.0.11");
  b.Entity("ControleGastos.Api.Models.Pessoa", e=>{e.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");e.Property<int>("Idade").HasColumnType("INTEGER");e.Property<string>("Nome").IsRequired().HasMaxLength(150).HasColumnType("TEXT");e.HasKey("Id");e.ToTable("Pessoas");});
  b.Entity("ControleGastos.Api.Models.Transacao", e=>{e.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");e.Property<int>("Categoria").HasColumnType("INTEGER");e.Property<DateTime>("Data").HasColumnType("TEXT");e.Property<string>("Descricao").IsRequired().HasMaxLength(250).HasColumnType("TEXT");e.Property<int>("PessoaId").HasColumnType("INTEGER");e.Property<int>("Tipo").HasColumnType("INTEGER");e.Property<decimal>("Valor").HasPrecision(18,2).HasColumnType("TEXT");e.HasKey("Id");e.HasIndex("Categoria");e.HasIndex("Data");e.HasIndex("PessoaId");e.ToTable("Transacoes");e.HasOne("ControleGastos.Api.Models.Pessoa","Pessoa").WithMany("Transacoes").HasForeignKey("PessoaId").OnDelete(DeleteBehavior.Cascade).IsRequired();});
 }
}
