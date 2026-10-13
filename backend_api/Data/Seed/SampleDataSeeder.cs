using EnglishRpg.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishRpg.Api.Data.Seed;

public static class SampleDataSeeder
{
    private static readonly University[] Universities =
    [
        new() { Name = "ĐH Quốc gia Hà Nội", ShortName = "VNU", ExpRequired = 10000 },
        new() { Name = "ĐH Quốc gia TP.HCM", ShortName = "VNU-HCM", ExpRequired = 10000 },
        new() { Name = "ĐH Bách khoa Hà Nội", ShortName = "HUST", ExpRequired = 9000 },
        new() { Name = "ĐH Bách khoa, ĐHQG TP.HCM", ShortName = "HCMUT", ExpRequired = 8000 },
        new() { Name = "ĐH Y Hà Nội", ShortName = "HMU", ExpRequired = 7500 },
        new() { Name = "ĐH Y Dược TP.HCM", ShortName = "UMP", ExpRequired = 7000 },
        new() { Name = "ĐH Ngoại thương", ShortName = "FTU", ExpRequired = 5000 },
        new() { Name = "ĐH Kinh tế TP.HCM", ShortName = "UEH", ExpRequired = 4500 }
    ];

    private static readonly Stage[] Stages =
    [
        new() { OrderNo = 1, Name = "Ải 1: Khởi đầu", Type = StageType.Normal, QuestionCount = 10, PassCorrect = 8, ExpPerCorrect = 200, BonusExp = 0 },
        new() { OrderNo = 2, Name = "Ải 2: Thử thách", Type = StageType.Normal, QuestionCount = 10, PassCorrect = 8, ExpPerCorrect = 300, BonusExp = 0 },
        new() { OrderNo = 3, Name = "Ải 3: Bứt phá", Type = StageType.Normal, QuestionCount = 10, PassCorrect = 8, ExpPerCorrect = 400, BonusExp = 0 },
        new() { OrderNo = 4, Name = "Boss", Type = StageType.Boss, QuestionCount = 15, PassCorrect = 13, TimeLimitSeconds = 1800, ExpPerCorrect = 0, BonusExp = 2000 }
    ];

    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var existingUniversityShortNames = await dbContext.Universities
            .Select(university => university.ShortName)
            .ToListAsync(cancellationToken);
        var universityShortNames = existingUniversityShortNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        dbContext.Universities.AddRange(Universities.Where(university => !universityShortNames.Contains(university.ShortName)));
        await dbContext.SaveChangesAsync(cancellationToken);

        var existingStages = await dbContext.Stages
            .Select(stage => new { stage.OrderNo, stage.Type })
            .ToListAsync(cancellationToken);
        var stageKeys = existingStages.Select(stage => (stage.OrderNo, stage.Type)).ToHashSet();
        dbContext.Stages.AddRange(Stages.Where(stage => !stageKeys.Contains((stage.OrderNo, stage.Type))));
        await dbContext.SaveChangesAsync(cancellationToken);

        var stagesByOrder = await dbContext.Stages
            .Where(stage => stage.OrderNo >= 1 && stage.OrderNo <= 4)
            .ToDictionaryAsync(stage => stage.OrderNo, cancellationToken);

        var questions = new[]
        {
            new Question
            {
                StageId = stagesByOrder[1].Id,
                OrderNo = 1,
                Content = "Choose the correct form: She ___ to school every day.",
                OptionA = "go",
                OptionB = "goes",
                OptionC = "going",
                OptionD = "gone",
                CorrectOption = "B",
                Hint1 = "The subject is third-person singular.",
                Hint2 = "Use the present simple form ending in -s.",
                Explanation = "In the present simple, a third-person singular subject takes a verb ending in -s."
            },
            new Question
            {
                StageId = stagesByOrder[1].Id,
                OrderNo = 2,
                Content = "Choose the word closest in meaning to 'begin'.",
                OptionA = "start",
                OptionB = "finish",
                OptionC = "forget",
                OptionD = "leave",
                CorrectOption = "A",
                Hint1 = "It means to do the first part of something.",
                Hint2 = "It is a synonym for 'commence'.",
                Explanation = "'Start' and 'begin' have similar meanings."
            },
            new Question
            {
                StageId = stagesByOrder[2].Id,
                OrderNo = 1,
                Content = "If I ___ more time, I would learn another language.",
                OptionA = "have",
                OptionB = "had",
                OptionC = "will have",
                OptionD = "am having",
                CorrectOption = "B",
                Hint1 = "This is a hypothetical situation.",
                Hint2 = "Use the past simple in the if-clause of the second conditional.",
                Explanation = "The second conditional uses if + past simple, followed by would + base verb."
            },
            new Question
            {
                StageId = stagesByOrder[2].Id,
                OrderNo = 2,
                Content = "The report ___ by the team yesterday.",
                OptionA = "completes",
                OptionB = "is completed",
                OptionC = "was completed",
                OptionD = "has completing",
                CorrectOption = "C",
                Hint1 = "The sentence is passive and refers to a finished past event.",
                Hint2 = "Use was/were + past participle.",
                Explanation = "The past simple passive is formed with was/were + past participle."
            },
            new Question
            {
                StageId = stagesByOrder[3].Id,
                OrderNo = 1,
                Content = "Despite ___ tired, he continued working.",
                OptionA = "be",
                OptionB = "was",
                OptionC = "being",
                OptionD = "to be",
                CorrectOption = "C",
                Hint1 = "'Despite' is followed by a noun or gerund.",
                Hint2 = "Use the -ing form of 'be'.",
                Explanation = "'Despite' is a preposition, so it can be followed by a gerund."
            },
            new Question
            {
                StageId = stagesByOrder[3].Id,
                OrderNo = 2,
                Content = "The new policy is expected to ___ a significant impact.",
                OptionA = "do",
                OptionB = "make",
                OptionC = "take",
                OptionD = "give",
                CorrectOption = "B",
                Hint1 = "Think of the common collocation with 'an impact'.",
                Hint2 = "The collocation is 'make an impact'.",
                Explanation = "The standard collocation is 'make an impact'."
            },
            new Question
            {
                StageId = stagesByOrder[4].Id,
                OrderNo = 1,
                Content = "Choose the correct word: The results were ___ with our expectations.",
                OptionA = "consistent",
                OptionB = "consisted",
                OptionC = "consistency",
                OptionD = "consist",
                CorrectOption = "A",
                Explanation = "'Consistent with' means in agreement with something."
            },
            new Question
            {
                StageId = stagesByOrder[4].Id,
                OrderNo = 2,
                Content = "Choose the correct connector: ___ the heavy rain, the event continued.",
                OptionA = "Although",
                OptionB = "Because",
                OptionC = "Despite",
                OptionD = "Unless",
                CorrectOption = "C",
                Explanation = "'Despite' is followed by a noun phrase and expresses contrast."
            }
        };

        foreach (var question in questions)
        {
            var alreadySeeded = await dbContext.Questions.AnyAsync(
                existing => existing.StageId == question.StageId && existing.OrderNo == question.OrderNo,
                cancellationToken);
            if (!alreadySeeded)
            {
                dbContext.Questions.Add(question);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
