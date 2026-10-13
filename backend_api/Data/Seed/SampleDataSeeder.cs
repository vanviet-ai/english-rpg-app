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

        var additionalQuestions = new[]
        {
            CreateSampleQuestion(stagesByOrder[1].Id, 3, "They ___ football after school.", ["play", "plays", "playing", "played"], "A", "The subject is plural.", "Use the base form of the verb with 'they'."),
            CreateSampleQuestion(stagesByOrder[1].Id, 4, "I ___ a book at the moment.", ["read", "reads", "am reading", "reading"], "C", "The action is happening now.", "Use the present continuous: am + verb-ing."),
            CreateSampleQuestion(stagesByOrder[1].Id, 5, "There ___ two apples on the table.", ["is", "are", "was", "be"], "B", "The noun is plural.", "Use 'are' with a plural noun in the present tense."),
            CreateSampleQuestion(stagesByOrder[1].Id, 6, "We went ___ the park yesterday.", ["at", "on", "to", "in"], "C", "Choose the preposition used with 'go' and a destination.", "The usual phrase is 'go to the park'."),
            CreateSampleQuestion(stagesByOrder[1].Id, 7, "This bag is ___ than that one.", ["heavy", "heavier", "heaviest", "more heavy"], "B", "Compare two things.", "Use the comparative form of this short adjective."),
            CreateSampleQuestion(stagesByOrder[1].Id, 8, "She has ___ finished her homework.", ["yet", "already", "ever", "ago"], "B", "The sentence says the task is finished earlier than expected.", "Use 'already' in an affirmative present-perfect sentence."),
            CreateSampleQuestion(stagesByOrder[1].Id, 9, "How ___ milk do you need?", ["many", "much", "few", "several"], "B", "Milk is uncountable.", "Use 'how much' with an uncountable noun."),
            CreateSampleQuestion(stagesByOrder[1].Id, 10, "The children ___ in the garden now.", ["plays", "is playing", "are playing", "played"], "C", "The subject is plural and the action is happening now.", "Use are + verb-ing."),
            CreateSampleQuestion(stagesByOrder[2].Id, 3, "By the time we arrived, the film ___.", ["starts", "has started", "had started", "will start"], "C", "One past event happened before another.", "Use the past perfect for the earlier past event."),
            CreateSampleQuestion(stagesByOrder[2].Id, 4, "She suggested ___ the meeting until Friday.", ["to postpone", "postpone", "postponing", "postponed"], "C", "The verb 'suggest' can be followed by a gerund.", "Use the -ing form after 'suggest'."),
            CreateSampleQuestion(stagesByOrder[2].Id, 5, "Neither of the answers ___ correct.", ["are", "were", "is", "have"], "C", "The subject begins with 'neither of'.", "In formal usage, 'neither' takes a singular verb."),
            CreateSampleQuestion(stagesByOrder[2].Id, 6, "If she had left earlier, she ___ the train.", ["would catch", "would have caught", "will catch", "caught"], "B", "This is an unreal condition in the past.", "Use would have + past participle in the result clause."),
            CreateSampleQuestion(stagesByOrder[2].Id, 7, "The manager asked where I ___.", ["live", "am living", "lived", "will live"], "C", "Reported speech usually shifts the tense back.", "The present simple commonly shifts to the past simple."),
            CreateSampleQuestion(stagesByOrder[2].Id, 8, "We are looking forward to ___ you.", ["see", "seeing", "be seen", "have seen"], "B", "Here, 'to' is a preposition.", "A preposition is followed by a gerund."),
            CreateSampleQuestion(stagesByOrder[2].Id, 9, "The problem was ___ difficult that nobody solved it.", ["so", "such", "too", "enough"], "A", "Use the pattern with an adjective followed by 'that'.", "The pattern is 'so + adjective + that'."),
            CreateSampleQuestion(stagesByOrder[2].Id, 10, "You ___ have told me; I already knew.", ["mustn't", "needn't", "couldn't", "shouldn't"], "B", "The action was unnecessary.", "Use 'needn't have' for an action that was done but was not necessary."),
            CreateSampleQuestion(stagesByOrder[3].Id, 3, "Rarely ___ such a convincing performance.", ["I have seen", "have I seen", "I saw", "I had saw"], "B", "A negative adverb at the start triggers inversion.", "Place the auxiliary before the subject."),
            CreateSampleQuestion(stagesByOrder[3].Id, 4, "The proposal, ___ by the committee, will go to a vote.", ["reviewing", "reviewed", "having review", "was reviewed"], "B", "The proposal receives the action.", "Use a past participle reduced relative clause."),
            CreateSampleQuestion(stagesByOrder[3].Id, 5, "Had it not been for your advice, I ___ the contract.", ["would sign", "would have signed", "will sign", "signed"], "B", "This is an inverted third conditional.", "The result clause takes would have + past participle."),
            CreateSampleQuestion(stagesByOrder[3].Id, 6, "The evidence is consistent ___ the witness's account.", ["to", "for", "with", "about"], "C", "Recall the preposition that follows 'consistent'.", "The collocation is 'consistent with'."),
            CreateSampleQuestion(stagesByOrder[3].Id, 7, "No sooner ___ the announcement than the crowd cheered.", ["did they hear", "they heard", "had they heard", "they had hear"], "C", "The phrase 'no sooner' at the beginning requires inversion.", "Use past perfect inversion: had + subject + participle."),
            CreateSampleQuestion(stagesByOrder[3].Id, 8, "Were the data ___, the conclusion would be different.", ["reliable", "reliably", "reliability", "rely"], "A", "The blank follows 'were' and describes the data.", "A linking verb is followed by an adjective."),
            CreateSampleQuestion(stagesByOrder[3].Id, 9, "The findings are worth ___ in greater detail.", ["to examine", "examining", "examined", "examine"], "B", "The expression 'worth' is followed by a gerund.", "Use the -ing form after 'worth'."),
            CreateSampleQuestion(stagesByOrder[3].Id, 10, "Only after the review ___ the scale of the problem.", ["we understood", "did we understand", "we did understood", "understood we"], "B", "A restrictive phrase at the beginning triggers inversion.", "Use auxiliary 'did' before the subject and the base verb.")
        };

        var bossQuestions = new[]
        {
            CreateBossQuestion(stagesByOrder[4].Id, 3, "He ___ breakfast at seven every day.", ["eat", "eats", "eating", "ate"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 4, "They ___ at home yesterday.", ["was", "are", "were", "be"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 5, "We ___ TV when the phone rang.", ["watch", "watched", "were watching", "are watching"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 6, "I have lived here ___ 2020.", ["for", "since", "during", "at"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 7, "There isn't ___ sugar in my tea.", ["many", "few", "much", "several"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 8, "She is good ___ drawing.", ["at", "in", "on", "for"], "A"),
            CreateBossQuestion(stagesByOrder[4].Id, 9, "Please put the book on ___ table.", ["a", "an", "the", "some"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 10, "If it rains, we ___ at home.", ["stay", "stayed", "will stay", "would stayed"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 11, "Tom is taller ___ Ben.", ["then", "than", "that", "as"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 12, "Please speak more ___.", ["slow", "slower", "slowly", "slowlier"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 13, "The opposite of 'early' is ___.", ["late", "fast", "near", "soon"], "A"),
            CreateBossQuestion(stagesByOrder[4].Id, 14, "I ___ my keys. Have you seen them?", ["lose", "lost", "have lost", "am losing"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 15, "She ___ to music now.", ["listens", "is listening", "listened", "listen"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 16, "We enjoyed ___ the museum.", ["visit", "to visit", "visiting", "visited"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 17, "You ___ wear a seat belt in a car.", ["must", "might", "would", "could"], "A"),
            CreateBossQuestion(stagesByOrder[4].Id, 18, "Can you ___ me your pen?", ["borrow", "lend", "take", "bring"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 19, "I was tired, ___ I went to bed.", ["but", "because", "so", "although"], "C"),
            CreateBossQuestion(stagesByOrder[4].Id, 20, "There are ___ oranges than apples.", ["less", "fewer", "little", "fewest"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 21, "He has ___ finished his homework.", ["yet", "just", "ever", "ago"], "B"),
            CreateBossQuestion(stagesByOrder[4].Id, 22, "What ___ you doing at eight last night?", ["was", "are", "were", "did"], "C")
        };

        questions = questions.Concat(additionalQuestions).Concat(bossQuestions).ToArray();

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

    private static Question CreateSampleQuestion(
        int stageId,
        int orderNo,
        string content,
        string[] options,
        string correctOption,
        string hint1,
        string hint2) =>
        new()
        {
            StageId = stageId,
            OrderNo = orderNo,
            Content = content,
            OptionA = options[0],
            OptionB = options[1],
            OptionC = options[2],
            OptionD = options[3],
            CorrectOption = correctOption,
            Hint1 = hint1,
            Hint2 = hint2
        };

    private static Question CreateBossQuestion(
        int stageId,
        int orderNo,
        string content,
        string[] options,
        string correctOption) =>
        new()
        {
            StageId = stageId,
            OrderNo = orderNo,
            Content = content,
            OptionA = options[0],
            OptionB = options[1],
            OptionC = options[2],
            OptionD = options[3],
            CorrectOption = correctOption
        };
}
