using EnglishRpg.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishRpg.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<University> Universities => Set<University>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<PlayerStageProgress> PlayerStageProgress => Set<PlayerStageProgress>();
    public DbSet<StageAttempt> StageAttempts => Set<StageAttempt>();
    public DbSet<AttemptQuestion> AttemptQuestions => Set<AttemptQuestion>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<University>(entity =>
        {
            entity.ToTable("Universities");
            entity.HasKey(university => university.Id);
            entity.Property(university => university.Name).HasMaxLength(200).IsRequired();
            entity.Property(university => university.ShortName).HasMaxLength(50).IsRequired();
            entity.HasMany(university => university.Players)
                .WithOne(player => player.TargetUniversity)
                .HasForeignKey(player => player.TargetUniversityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Players");
            entity.HasKey(player => player.Id);
            entity.HasIndex(player => player.Name).IsUnique();
            entity.Property(player => player.Name).HasMaxLength(100).IsRequired();
            entity.Property(player => player.CreatedAt).HasColumnType("datetime2");
        });

        modelBuilder.Entity<Stage>(entity =>
        {
            entity.ToTable("Stages");
            entity.HasKey(stage => stage.Id);
            entity.Property(stage => stage.Name).HasMaxLength(100).IsRequired();
            entity.Property(stage => stage.Type).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(question => question.Id);
            entity.Property(question => question.Content).IsRequired();
            entity.Property(question => question.OptionA).HasMaxLength(1000).IsRequired();
            entity.Property(question => question.OptionB).HasMaxLength(1000).IsRequired();
            entity.Property(question => question.OptionC).HasMaxLength(1000).IsRequired();
            entity.Property(question => question.OptionD).HasMaxLength(1000).IsRequired();
            entity.Property(question => question.CorrectOption).HasMaxLength(1).IsRequired();
            entity.Property(question => question.Hint1).IsRequired(false);
            entity.Property(question => question.Hint2).IsRequired(false);
            entity.Property(question => question.Explanation).IsRequired(false);
            entity.HasOne(question => question.Stage)
                .WithMany(stage => stage.Questions)
                .HasForeignKey(question => question.StageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlayerStageProgress>(entity =>
        {
            entity.ToTable("PlayerStageProgress");
            entity.HasKey(progress => new { progress.PlayerId, progress.StageId });
            entity.HasOne(progress => progress.Player)
                .WithMany(player => player.StageProgress)
                .HasForeignKey(progress => progress.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(progress => progress.Stage)
                .WithMany(stage => stage.PlayerProgress)
                .HasForeignKey(progress => progress.StageId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(progress => progress.FirstClearedAt).HasColumnType("datetime2");
        });

        modelBuilder.Entity<StageAttempt>(entity =>
        {
            entity.ToTable("StageAttempts");
            entity.HasKey(attempt => attempt.Id);
            entity.Property(attempt => attempt.StartedAt).HasColumnType("datetime2");
            entity.Property(attempt => attempt.ExpiresAt).HasColumnType("datetime2");
            entity.Property(attempt => attempt.FinishedAt).HasColumnType("datetime2");
            entity.Property(attempt => attempt.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(attempt => attempt.Player)
                .WithMany(player => player.StageAttempts)
                .HasForeignKey(attempt => attempt.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(attempt => attempt.Stage)
                .WithMany(stage => stage.Attempts)
                .HasForeignKey(attempt => attempt.StageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AttemptQuestion>(entity =>
        {
            entity.ToTable("AttemptQuestions");
            entity.HasKey(attemptQuestion => new { attemptQuestion.AttemptId, attemptQuestion.QuestionId });
            entity.HasOne(attemptQuestion => attemptQuestion.Attempt)
                .WithMany(attempt => attempt.AttemptQuestions)
                .HasForeignKey(attemptQuestion => attemptQuestion.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(attemptQuestion => attemptQuestion.Question)
                .WithMany(question => question.AttemptQuestions)
                .HasForeignKey(attemptQuestion => attemptQuestion.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AttemptAnswer>(entity =>
        {
            entity.ToTable("AttemptAnswers");
            entity.HasKey(answer => answer.Id);
            entity.Property(answer => answer.SelectedOption).HasMaxLength(1).IsRequired(false);
            entity.HasOne(answer => answer.Attempt)
                .WithMany(attempt => attempt.AttemptAnswers)
                .HasForeignKey(answer => answer.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(answer => answer.Question)
                .WithMany(question => question.AttemptAnswers)
                .HasForeignKey(answer => answer.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
