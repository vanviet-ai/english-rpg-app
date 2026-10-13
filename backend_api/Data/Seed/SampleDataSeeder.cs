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

    private static readonly SeedQuestion[] Stage1Questions =
    [
        Q("Choose the correct form: Lan ___ English every evening.", ["study", "studies", "studied", "studying"], "B", "The present simple takes -ies with a third-person singular verb ending in consonant + y.", "Check whether the subject is third-person singular and how the verb ending changes.", "Use the present simple form 'studies'."),
        Q("My parents ___ coffee in the morning.", ["doesn't drink", "don't drink", "not drinks", "didn't drinks"], "B", "Use do not with the plural subject 'my parents'; the main verb stays in its base form.", "The subject is plural, so choose the matching negative auxiliary.", "Use 'don't' followed by the base verb 'drink'."),
        Q("___ your brother ___ football last Sunday?", ["Does / play", "Did / played", "Did / play", "Was / play"], "C", "In a past-simple question, did is followed by the base form of the main verb.", "The time expression 'last Sunday' signals the past; check the verb after the auxiliary.", "Use 'Did ... play'."),
        Q("We ___ our grandparents yesterday.", ["visit", "visits", "visited", "have visit"], "C", "A completed action at a definite time in the past takes the past simple.", "Look at the time expression 'yesterday'.", "Use the past form 'visited'."),
        Q("There ___ three ___ in the classroom.", ["is / child", "are / children", "is / children", "are / childs"], "B", "A plural subject takes 'are'; the plural of child is irregular.", "Check both subject-verb agreement and the irregular plural form.", "Use 'are' and 'children'."),
        Q("Two ___ are waiting outside.", ["woman", "womans", "women", "womens"], "C", "The plural of 'woman' changes its internal vowel.", "This is an irregular plural, not a regular -s plural.", "The plural form is 'women'."),
        Q("The ___ on the shelf ___ mine.", ["book / are", "books / is", "books / are", "book / were"], "C", "The plural noun 'books' agrees with 'are'.", "Both the noun and the verb must be plural.", "Use 'books are'."),
        Q("Mai ___ her homework before dinner every day.", ["finish", "finishes", "finished", "finishing"], "B", "The present-simple verb needs the third-person singular ending.", "The subject is 'Mai', and 'every day' describes a routine.", "Use 'finishes'."),
        Q("They ___ to the museum last weekend.", ["go", "goes", "went", "gone"], "C", "The sentence describes a completed event in the past.", "Notice 'last weekend' and use the past form of 'go'.", "The past simple of 'go' is 'went'."),
        Q("Each student ___ a dictionary.", ["have", "has", "are having", "were having"], "B", "'Each student' is grammatically singular and takes a singular verb.", "Although the noun refers to many people, 'each' makes the subject singular.", "Use 'has'.")
    ];

    private static readonly SeedQuestion[] Stage2Questions =
    [
        Q("I ___ this novel twice, so I know the ending.", ["read", "have read", "am reading", "had read"], "B", "The experience is relevant now and no finished past time is stated.", "Choose the present perfect form of 'read' (past participle: read).", "Use 'have read' for life experience up to the present."),
        Q("She ___ in Da Nang since 2021.", ["lives", "lived", "has lived", "is living"], "C", "'Since 2021' gives the starting point of a situation continuing to now.", "Use a perfect tense with 'since' for a continuing state.", "Use the present perfect 'has lived'."),
        Q("Have you finished the report ___?", ["already", "yet", "since", "for"], "B", "In a present-perfect question asking whether something is complete, use a word commonly placed at the end.", "This word is often used at the end of questions and negatives.", "Use 'yet'."),
        Q("This exercise is ___ than the previous one.", ["more difficult", "most difficult", "difficultest", "the more difficult"], "A", "The sentence compares two exercises.", "Use the comparative form of a multi-syllable adjective followed by 'than'.", "Use 'more difficult'."),
        Q("Of the three routes, this one is ___.", ["shorter", "the shortest", "shortest", "the most short"], "B", "The sentence compares one item with a group of three.", "Use the superlative form and include the definite article.", "Use 'the shortest'."),
        Q("My sister is interested ___ learning Japanese.", ["on", "at", "in", "for"], "C", "The adjective 'interested' takes a fixed preposition before a noun or gerund.", "Recall the collocation 'interested ___ something'.", "The correct phrase is 'interested in'."),
        Q("We have known each other ___ ten years.", ["since", "during", "for", "from"], "C", "The phrase gives a duration, not a starting date.", "Use the preposition that goes with a length of time.", "Use 'for' before 'ten years'."),
        Q("The flight arrived ___ the airport at noon.", ["in", "at", "on", "to"], "B", "Use the preposition for arriving at a specific point or place.", "The airport is treated as a specific destination here.", "The standard phrase is 'arrive at the airport'."),
        Q("This is the ___ film I have seen this year.", ["better", "best", "good", "well"], "B", "The sentence ranks the film against all the films seen this year.", "Choose the superlative form of the irregular adjective 'good'.", "The superlative of 'good' is 'best'."),
        Q("The new library is much ___ than the old one.", ["modern", "more modern", "most modern", "the modernest"], "B", "The word 'than' signals a comparison between two libraries.", "Use 'more' with this multi-syllable adjective in the comparative.", "Use 'more modern'.")
    ];

    private static readonly SeedQuestion[] Stage3Questions =
    [
        Q("If I ___ enough money, I would travel around the world.", ["have", "had", "will have", "would have"], "B", "This is an unreal or hypothetical situation in the present.", "In the second conditional, use the past simple in the if-clause.", "Use 'had': if + past simple, would + base verb."),
        Q("If they had booked earlier, they ___ a better price.", ["get", "would get", "would have got", "will get"], "C", "The condition refers to an unreal situation in the past.", "Use the third conditional result form: would have + past participle.", "Use 'would have got'."),
        Q("The scientist ___ discovered the new method received an award.", ["which", "whose", "who", "where"], "C", "The relative pronoun refers to a person and is the subject of the clause.", "Choose the subject relative pronoun for a person.", "Use 'who'."),
        Q("This is the school ___ my mother used to teach.", ["who", "where", "whose", "which"], "B", "The missing relative word refers to a place where an action happened.", "Use a relative adverb to mean 'at/in which'.", "Use 'where'."),
        Q("The invitations ___ to all the guests yesterday.", ["sent", "were sent", "are sending", "have send"], "B", "The sentence is passive and describes a completed event yesterday.", "Form the past simple passive with was/were + past participle.", "Use 'were sent'."),
        Q("All applications must ___ before 5 p.m. on Friday.", ["submit", "be submitted", "submitted", "be submitting"], "B", "The subject receives the action, and the modal is followed by a passive infinitive.", "After 'must', use 'be' plus a past participle for the passive.", "Use 'must be submitted'."),
        Q("If it ___ tomorrow, we will hold the meeting online.", ["rained", "will rain", "rains", "would rain"], "C", "This is a possible future condition.", "The first conditional uses the present simple in its if-clause.", "Use 'rains'."),
        Q("The laptop, ___ I bought last month, has stopped working.", ["who", "where", "which", "whose"], "C", "The relative pronoun refers to a thing and is the object of the clause.", "Choose the object relative pronoun for an inanimate noun.", "Use 'which'."),
        Q("The old bridge ___ next year.", ["will replace", "will be replaced", "is replacing", "has replaced"], "B", "The bridge receives the action, and the time is in the future.", "Use will + be + past participle for the future passive.", "Use 'will be replaced'."),
        Q("If she ___ the instructions carefully, she would not have made that mistake.", ["read", "had read", "would read", "has read"], "B", "The result clause refers to a past outcome that did not happen.", "Use the past perfect in the if-clause of a third conditional.", "Use 'had read'.")
    ];

    private static readonly SeedQuestion[] BossQuestions =
    [
        Q("By the time we reached the station, the train ___.", ["leaves", "has left", "had left", "was leaving"], "C", "The train's departure happened before another past event.", "Use the past perfect for the earlier of two past actions.", "The train had already left before we reached the station."),
        Q("The woman ___ lives next door is a doctor.", ["which", "who", "whose", "where"], "B", "The relative pronoun refers to a person and is the subject of the clause.", "Choose the subject relative pronoun for a person.", "Use 'who' for a person."),
        Q("Neither the teacher nor the students ___ ready for the trip.", ["was", "is", "were", "has been"], "C", "With 'neither...nor', the verb agrees with the nearer subject, 'students'.", "Check the noun immediately before the verb.", "The nearer subject is plural, so use 'were'."),
        Q("The room ___ before the guests arrived.", ["had cleaned", "had been cleaned", "has cleaned", "was cleaning"], "B", "The room receives the action, which was completed before another past event.", "Use the past perfect passive: had been + past participle.", "Use 'had been cleaned'."),
        Q("If I ___ about the traffic, I would have left earlier.", ["know", "knew", "had known", "would know"], "C", "This is an unreal condition referring to the past.", "The if-clause of the third conditional uses the past perfect.", "Use 'had known'."),
        Q("The more carefully you plan, ___ the result will be.", ["good", "the better", "best", "the best"], "B", "This uses the paired comparative structure 'the more..., the ...'.", "Complete the second half with a comparative, including 'the'.", "Use 'the better'."),
        Q("She has worked at this hospital ___ five years.", ["since", "during", "for", "from"], "C", "The phrase states a duration.", "Choose the preposition used before a period of time.", "Use 'for' before 'five years'."),
        Q("The book ___ cover is blue belongs to Minh.", ["who", "whose", "which", "where"], "B", "The blank shows possession, linking the book to its cover.", "Use the possessive relative pronoun.", "Use 'whose'."),
        Q("Hardly ___ the house when it began to snow.", ["we had left", "had we left", "we left", "did we left"], "B", "A sentence beginning with 'hardly' uses inversion.", "Put the past-perfect auxiliary before the subject.", "Use 'Hardly had we left'."),
        Q("The road is closed because it ___.", ["repairs", "is repairing", "is being repaired", "has repaired"], "C", "The road receives an action that is in progress now.", "Use the present continuous passive.", "Use 'is being repaired'."),
        Q("I wish I ___ more time to prepare for the exam.", ["have", "had", "will have", "would have"], "B", "This expresses a wish about a present situation that is not true.", "After 'I wish', use a past form for an unreal present.", "Use 'had'."),
        Q("Despite ___ tired, he continued working.", ["be", "being", "was", "to be"], "B", "'Despite' is a preposition and can be followed by a gerund.", "Use the -ing form after this preposition.", "Use 'being'."),
        Q("The manager asked me where I ___.", ["live", "am living", "lived", "will live"], "C", "The question is reported in the past, so the tense commonly shifts back.", "Use past-tense backshift in reported speech.", "Use 'lived'."),
        Q("She is one of the most talented ___ in the class.", ["student", "students", "student's", "students'"], "B", "'One of the' is followed by a plural countable noun.", "The phrase refers to one person from a group.", "Use the plural 'students'."),
        Q("The meeting was postponed ___ the director was ill.", ["because", "although", "despite", "unless"], "A", "A clause gives the reason for postponing the meeting.", "Choose a conjunction that can introduce a full clause of reason.", "Use 'because'."),
        Q("The new bridge is expected ___ next year.", ["complete", "to complete", "to be completed", "being completed"], "C", "The bridge receives the action in a future expectation.", "Use a passive infinitive after 'is expected'.", "Use 'to be completed'."),
        Q("Please speak ___ so that everyone can understand you.", ["clear", "clearly", "clearness", "clearing"], "B", "The blank modifies the verb 'speak'.", "Choose the adverb form of 'clear'.", "Use 'clearly'."),
        Q("We have not seen our cousins ___ they moved to Hue.", ["for", "while", "since", "during"], "C", "The second clause gives the starting point of a period continuing to now.", "Use the preposition that introduces a starting event or time.", "Use 'since'."),
        Q("The more you practise, ___ you become.", ["the confident", "the more confident", "more confidently", "most confident"], "B", "This is a paired comparative structure, and 'become' is followed by an adjective.", "Use 'the more' with a multi-syllable adjective in the second clause.", "Use 'the more confident'."),
        Q("The woman to ___ I spoke was very helpful.", ["who", "whom", "whose", "which"], "B", "The relative pronoun is the object of the preposition 'to'.", "Formal English uses the object form after a preposition.", "Use 'whom'."),
        Q("Not only ___ the competition, but she also broke the record.", ["she won", "did she win", "she did won", "won she"], "B", "When 'not only' begins a clause, subject-auxiliary inversion is used.", "Use 'did' before the subject and the base form of the verb.", "Use 'did she win'."),
        Q("The instructions ___ clearly before the experiment began.", ["explained", "were explained", "have explained", "are explaining"], "B", "The instructions receive the action in a completed past event.", "Use the past simple passive.", "Use 'were explained'."),
        Q("If he ___ harder, he would be a better player now.", ["practises", "practised", "had practised", "will practise"], "B", "This is a second conditional describing a hypothetical present.", "Use the past simple in the if-clause.", "Use 'practised'."),
        Q("She apologized ___ being late for the interview.", ["to", "for", "about", "with"], "B", "The verb 'apologize' takes a fixed preposition before a gerund.", "Recall the collocation 'apologize ___ something'.", "Use 'for'."),
        Q("The village ___ we spent our holiday is near the coast.", ["which", "whose", "where", "who"], "C", "The relative word refers to a place where an action happened.", "Use a relative adverb meaning 'in which'.", "Use 'where'."),
        Q("You ___ use your phone during the examination; it is prohibited.", ["don't have to", "mustn't", "couldn't", "wouldn't"], "B", "The sentence expresses a prohibition, not a lack of necessity.", "Choose the modal meaning 'must not'.", "'Mustn't' expresses prohibition."),
        Q("The project ___ by the end of this month.", ["will complete", "will be completed", "is completing", "has completed"], "B", "The project receives the action, and the sentence refers to the future.", "Use the future simple passive.", "Use 'will be completed'."),
        Q("He is accustomed to ___ up early.", ["get", "got", "getting", "be getting"], "C", "In 'be accustomed to', 'to' is a preposition.", "A gerund follows this use of 'to'.", "Use 'getting'."),
        Q("The teacher, ___ advice I followed, congratulated me.", ["who", "whom", "whose", "which"], "C", "The blank indicates that the advice belongs to the teacher.", "Use a possessive relative pronoun before a noun.", "Use 'whose'."),
        Q("Unless you ___ now, you will miss the bus.", ["leave", "will leave", "left", "would leave"], "A", "An 'unless' clause can express a real future condition.", "Use the present simple in the conditional clause.", "Use 'leave'."),
        Q("I prefer reading at home ___ watching television.", ["than", "from", "to", "over"], "C", "The verb 'prefer' uses a fixed comparison pattern with two gerunds.", "The pattern is 'prefer A ___ B'.", "Use 'to'."),
        Q("The students were made ___ the test again.", ["take", "to take", "taking", "taken"], "B", "In the passive form of 'make', the infinitive marker is used.", "Compare passive 'be made ___' with active 'make someone do'.", "Use 'to take'."),
        Q("He has a good command ___ English.", ["at", "on", "of", "in"], "C", "This noun takes a fixed preposition when referring to a language or skill.", "Recall the phrase 'a command ___ a language'.", "Use 'of'."),
        Q("The match was cancelled ___ the heavy rain.", ["because", "because of", "although", "in spite"], "B", "The reason is expressed as a noun phrase, not a clause.", "Choose the expression followed by a noun phrase.", "Use 'because of'."),
        Q("By next June, they ___ the new hospital.", ["complete", "will complete", "will have completed", "are completing"], "C", "The action will be finished before a future time.", "Use the future perfect for completion before a specified future point.", "Use 'will have completed'."),
        Q("The evidence ___ by the police is being examined.", ["collecting", "collected", "was collected", "collect"], "B", "The evidence receives the action, and the clause is reduced.", "Use a past participle in a reduced passive relative clause.", "Use 'collected'."),
        Q("Neither of the proposals ___ acceptable to the board.", ["are", "were", "is", "have"], "C", "In formal English, 'neither of' is treated as singular.", "Use a singular verb with 'neither'.", "Use 'is'."),
        Q("She speaks English ___ than her brother.", ["fluent", "more fluently", "most fluently", "more fluent"], "B", "The blank modifies the verb 'speaks' and is compared with another person.", "Use the comparative form of the adverb 'fluently'.", "Use 'more fluently'."),
        Q("The house ___ roof was damaged has been repaired.", ["which", "whose", "who", "where"], "B", "The roof belongs to the house.", "Use the possessive relative pronoun before 'roof'.", "Use 'whose'."),
        Q("If water reaches 100°C, it ___.", ["boiled", "will boil", "boils", "would boil"], "C", "This is a general scientific fact.", "Use the zero conditional: present simple in both clauses.", "Use 'boils'."),
        Q("The suspect is believed ___ the country last night.", ["leave", "to leave", "to have left", "leaving"], "C", "The alleged departure happened before the present belief.", "Use a perfect infinitive for an earlier action.", "Use 'to have left'."),
        Q("She insisted ___ paying for the meal.", ["on", "at", "to", "for"], "A", "The verb 'insist' takes a fixed preposition before a gerund.", "Recall the pattern 'insist ___ doing something'.", "Use 'on'."),
        Q("The car needs ___ before the long journey.", ["service", "servicing", "to service", "serviced"], "B", "After 'need', an -ing form can express that something requires an action.", "The car receives the action; use the passive meaning of a gerund.", "Use 'servicing'."),
        Q("I will lend you my notes ___ you return them tomorrow.", ["as long as", "even though", "in case of", "despite"], "A", "The speaker sets a condition for lending the notes.", "Choose the conjunction meaning 'provided that'.", "Use 'as long as'."),
        Q("The students ___ essays were selected will represent the school.", ["who", "whom", "whose", "which"], "C", "The essays belong to the students.", "Use a possessive relative pronoun before a noun.", "Use 'whose'."),
        Q("Scarcely had the show begun ___ the lights went out.", ["than", "when", "then", "while"], "B", "The correlative expression is 'scarcely...when'.", "Complete the fixed pair used with past perfect inversion.", "Use 'when'."),
        Q("The medicine should ___ after meals.", ["take", "be taken", "have taken", "taking"], "B", "The medicine receives the action, and the modal is followed by a passive form.", "Use should + be + past participle.", "Use 'be taken'."),
        Q("He denied ___ the confidential document.", ["to copy", "copy", "copying", "copied"], "C", "The verb 'deny' is followed by a gerund.", "Choose the -ing form after 'deny'.", "Use 'copying'."),
        Q("This is the first time I ___ such a breathtaking view.", ["see", "saw", "have seen", "had seen"], "C", "The phrase 'This is the first time' commonly introduces a present-perfect clause.", "Use the present perfect for experience up to the present.", "Use 'have seen'."),
        Q("The more he practised, ___ he became.", ["the good", "the better", "better", "the best"], "B", "Both clauses describe change using a comparative structure.", "Complete 'the more..., the ...' with a comparative form.", "Use 'the better'."),
        Q("The company, ___ was founded in 1998, now employs 500 people.", ["that", "which", "what", "where"], "B", "This is a non-defining relative clause set off by commas.", "Use 'which' for a thing in a non-defining clause.", "Use 'which'; 'that' is not used in non-defining relative clauses."),
        Q("Had I known about the delay, I ___ earlier.", ["leave", "will leave", "would leave", "would have left"], "D", "This is an inverted third conditional; 'Had I known' means 'If I had known'.", "Use would have + past participle in the result clause.", "Use 'would have left'."),
        Q("The proposal was rejected, ___ surprised everyone.", ["who", "that", "which", "where"], "C", "The relative clause refers to the whole preceding statement.", "Use a non-defining relative pronoun for a preceding idea.", "Use 'which'."),
        Q("He is responsible ___ organizing the annual conference.", ["to", "of", "for", "with"], "C", "The adjective 'responsible' takes a fixed preposition before a gerund.", "Recall the pattern 'responsible ___ doing something'.", "Use 'for'."),
        Q("No sooner had the plane landed ___ the passengers stood up.", ["when", "then", "than", "while"], "C", "The correlative expression is 'no sooner...than'.", "Complete the fixed pair used with past perfect inversion.", "Use 'than'."),
        Q("The missing painting ___ in the basement yesterday.", ["found", "was found", "has found", "is finding"], "B", "The painting receives the action in a completed past event.", "Use the past simple passive.", "Use 'was found'."),
        Q("If she were more careful, she ___ fewer mistakes.", ["made", "will make", "would make", "would have made"], "C", "This describes an unreal or hypothetical present situation.", "Use would + base verb in the result clause of a second conditional.", "Use 'would make'."),
        Q("The athlete ___ won the gold medal thanked her coach.", ["which", "whose", "who", "whom"], "C", "The relative pronoun refers to a person and is the subject of the clause.", "Choose the subject relative pronoun.", "Use 'who'."),
        Q("Many endangered species ___ by habitat loss.", ["threaten", "are threatened", "have threatening", "are threatening"], "B", "The species receive the action, and the statement describes a present fact.", "Use the present simple passive.", "Use 'are threatened'."),
        Q("She would rather ___ at home tonight than go out.", ["stay", "to stay", "staying", "stayed"], "A", "The pattern 'would rather' is followed by a bare infinitive.", "Do not use 'to' after 'would rather'.", "Use 'stay'.")
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

        ValidateStage(stagesByOrder, 1, StageType.Normal);
        ValidateStage(stagesByOrder, 2, StageType.Normal);
        ValidateStage(stagesByOrder, 3, StageType.Normal);
        ValidateStage(stagesByOrder, 4, StageType.Boss);

        var seedQuestions = new List<SeededQuestion>();
        AddStageQuestions(seedQuestions, 1, Stage1Questions);
        AddStageQuestions(seedQuestions, 2, Stage2Questions);
        AddStageQuestions(seedQuestions, 3, Stage3Questions);
        AddStageQuestions(seedQuestions, 4, BossQuestions);

        var stageIds = stagesByOrder.Values.Select(stage => stage.Id).ToArray();
        var existingQuestions = await dbContext.Questions
            .Where(question => stageIds.Contains(question.StageId))
            .ToListAsync(cancellationToken);
        var duplicateQuestion = existingQuestions
            .GroupBy(question => (question.StageId, question.OrderNo))
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateQuestion is not null)
        {
            throw new InvalidOperationException(
                $"Duplicate seeded questions exist for stage {duplicateQuestion.Key.StageId}, order {duplicateQuestion.Key.OrderNo}.");
        }

        var questionsByKey = existingQuestions.ToDictionary(
            question => (question.StageId, question.OrderNo));
        foreach (var seededQuestion in seedQuestions)
        {
            var stageId = stagesByOrder[seededQuestion.StageOrder].Id;
            if (questionsByKey.TryGetValue((stageId, seededQuestion.OrderNo), out var existingQuestion))
            {
                ApplyQuestionData(existingQuestion, seededQuestion.Data);
                continue;
            }

            var question = new Question
            {
                StageId = stageId,
                OrderNo = seededQuestion.OrderNo,
                Content = seededQuestion.Data.Content,
                OptionA = seededQuestion.Data.Options[0],
                OptionB = seededQuestion.Data.Options[1],
                OptionC = seededQuestion.Data.Options[2],
                OptionD = seededQuestion.Data.Options[3],
                CorrectOption = seededQuestion.Data.CorrectOption,
                Hint1 = seededQuestion.Data.Hint1,
                Hint2 = seededQuestion.Data.Hint2,
                Explanation = seededQuestion.Data.Explanation
            };
            dbContext.Questions.Add(question);
            questionsByKey.Add((stageId, seededQuestion.OrderNo), question);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateStage(
        IReadOnlyDictionary<int, Stage> stagesByOrder,
        int orderNo,
        StageType expectedType)
    {
        if (!stagesByOrder.TryGetValue(orderNo, out var stage) || stage.Type != expectedType)
        {
            throw new InvalidOperationException(
                $"Expected stage {orderNo} to exist with type {expectedType} before seeding questions.");
        }
    }

    private static void AddStageQuestions(
        ICollection<SeededQuestion> destination,
        int stageOrder,
        IReadOnlyList<SeedQuestion> questions)
    {
        var expectedCount = stageOrder == 4 ? 60 : 10;
        if (questions.Count != expectedCount)
        {
            throw new InvalidOperationException(
                $"Stage {stageOrder} must define exactly {expectedCount} questions.");
        }

        for (var index = 0; index < questions.Count; index++)
        {
            var question = questions[index];
            if (stageOrder == 4)
            {
                question = question with { Hint1 = null, Hint2 = null };
            }

            if (string.IsNullOrWhiteSpace(question.Explanation) ||
                (stageOrder < 4 &&
                 (string.IsNullOrWhiteSpace(question.Hint1) ||
                  string.IsNullOrWhiteSpace(question.Hint2))) ||
                (stageOrder == 4 && (question.Hint1 is not null || question.Hint2 is not null)))
            {
                throw new InvalidOperationException(
                    $"Question {index + 1} in stage {stageOrder} has missing or unexpected learning content.");
            }

            destination.Add(new SeededQuestion(stageOrder, index + 1, question));
        }
    }

    private static void ApplyQuestionData(Question question, SeedQuestion data)
    {
        question.Content = data.Content;
        question.OptionA = data.Options[0];
        question.OptionB = data.Options[1];
        question.OptionC = data.Options[2];
        question.OptionD = data.Options[3];
        question.CorrectOption = data.CorrectOption;
        question.Hint1 = data.Hint1;
        question.Hint2 = data.Hint2;
        question.Explanation = data.Explanation;
    }

    private static SeedQuestion Q(
        string content,
        string[] options,
        string correctOption,
        string explanation,
        string? hint1 = null,
        string? hint2 = null)
    {
        if (options.Length != 4 ||
            options.Any(string.IsNullOrWhiteSpace) ||
            options.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 4 ||
            string.IsNullOrWhiteSpace(content) ||
            string.IsNullOrWhiteSpace(explanation) ||
            correctOption is not ("A" or "B" or "C" or "D"))
        {
            throw new ArgumentException(
                "A question must have content, four distinct options, an explanation, and a valid correct option.");
        }

        return new SeedQuestion(content, options, correctOption, explanation, hint1, hint2);
    }

    private sealed record SeedQuestion(
        string Content,
        string[] Options,
        string CorrectOption,
        string Explanation,
        string? Hint1,
        string? Hint2);

    private sealed record SeededQuestion(int StageOrder, int OrderNo, SeedQuestion Data);
}
