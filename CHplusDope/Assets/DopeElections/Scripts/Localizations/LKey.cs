using Localizator;

namespace DopeElections.Localizations
{
    public static class LKey
    {
        public static class ScriptedSequences
        {
            public static class GrabSequence
            {
                public static readonly LocalizationKey Success
                    = new LocalizationKey("sequence.grab_sequence.success")
                    {
                        fallback = "Perfect!"
                    };

                public static readonly LocalizationKey Miss
                    = new LocalizationKey("sequence.grab_sequence.miss")
                    {
                        fallback = "Miss!"
                    };

                public static readonly LocalizationKey Tap
                    = new LocalizationKey("sequence.grab_sequence.tap") {fallback = "Tap!"};
            }

            public static class IntroComic
            {
                public static readonly LocalizationKey Dialogue01
                    = new LocalizationKey("sequence.intro_comic.dialogue_01") {fallback = "AAHARAH!"};

                public static readonly LocalizationKey Dialogue02
                    = new LocalizationKey("sequence.intro_comic.dialogue_02") {fallback = "AAHARAH!"};

                public static readonly LocalizationKey Dialogue03
                    = new LocalizationKey("sequence.intro_comic.dialogue_03") {fallback = "WHO HAS THE KEY!?"};

                public static readonly LocalizationKey Dialogue04
                    = new LocalizationKey("sequence.intro_comic.dialogue_04") {fallback = "LET ME IN PLEASE!"};

                public static readonly LocalizationKey Dialogue05
                    = new LocalizationKey("sequence.intro_comic.dialogue_05") {fallback = "PFF.."};

                public static readonly LocalizationKey Dialogue06
                    = new LocalizationKey("sequence.intro_comic.dialogue_06") {fallback = "SOOO MANY CANDIDATES..."};

                public static readonly LocalizationKey Dialogue07
                    = new LocalizationKey("sequence.intro_comic.dialogue_07") {fallback = "GRÜEZI!!"};

                public static readonly LocalizationKey Tap
                    = new LocalizationKey("sequence.intro_comic.tap") {fallback = "*TAP*"};

                public static readonly LocalizationKey Dialogue08
                    = new LocalizationKey("sequence.intro_comic.dialogue_08") {fallback = "WHAT?!"};

                public static readonly LocalizationKey Dialogue09
                    = new LocalizationKey("sequence.intro_comic.dialogue_09") {fallback = "WHAT'S THAT!?"};

                public static readonly LocalizationKey VotingRight
                    = new LocalizationKey("sequence.intro_comic.voting_right") {fallback = "WAHLRECHT!!"};

                public static readonly LocalizationKey Dialogue10
                    = new LocalizationKey("sequence.intro_comic.dialogue_10") {fallback = "WOW"};

                public static readonly LocalizationKey Dialogue11
                    = new LocalizationKey("sequence.intro_comic.dialogue_11") {fallback = "THE KEY!?"};

                public static readonly LocalizationKey Dialogue12
                    = new LocalizationKey("sequence.intro_comic.dialogue_12") {fallback = "GIVE ME THE KEY!"};

                public static readonly LocalizationKey Dialogue13
                    = new LocalizationKey("sequence.intro_comic.dialogue_13") {fallback = "IT'S MINE!"};

                public static readonly LocalizationKey Dialogue14
                    = new LocalizationKey("sequence.intro_comic.dialogue_14")
                        {fallback = "I ONLY WANT WHAT'S BEST FOR YOU!"};

                public static readonly LocalizationKey Dialogue15
                    = new LocalizationKey("sequence.intro_comic.dialogue_15") {fallback = "HAH! NICE TRY!"};

                public static readonly LocalizationKey Dialogue16
                    = new LocalizationKey("sequence.intro_comic.dialogue_16")
                        {fallback = "I WILL DECIDE WHO IS WORTHY OF THIS KEY!!"};

                public static readonly LocalizationKey Dialogue17
                    = new LocalizationKey("sequence.intro_comic.dialogue_17") {fallback = "I'LL DO ANYTHING!"};

                public static readonly LocalizationKey Dialogue18
                    = new LocalizationKey("sequence.intro_comic.dialogue_18") {fallback = "I'LL PROVE IT!"};

                public static readonly LocalizationKey Dialogue19
                    = new LocalizationKey("sequence.intro_comic.dialogue_19") {fallback = "ME!"};

                public static readonly LocalizationKey Dialogue20
                    = new LocalizationKey("sequence.intro_comic.dialogue_20") {fallback = "ME!"};

                public static readonly LocalizationKey Dialogue21
                    = new LocalizationKey("sequence.intro_comic.dialogue_21") {fallback = "ME!"};
            }

            public static class LandInMainMenu
            {
                public static readonly LocalizationKey WithMyVeryOwnRaceRoyale
                    = new LocalizationKey("sequence.land_in_main_menu.with_my_own_race_royale")
                        {fallback = "<uppercase>...in my very own\n<b><size=20>Race Royale</size></b></uppercase>"};
            }

            public static class PlanetChase
            {
                public static readonly LocalizationKey PutCharacterToTheTest
                    = new LocalizationKey("sequence.planet_chase.put_character_to_the_test")
                    {
                        fallback =
                            "<uppercase>Muhaha... Now, I will put\nyour <b>qualities</b> to the <b>test</b>!</uppercase>"
                    };
            }
        }

        public static class Components
        {
            public static class Answer
            {
                public static readonly LocalizationKey CompletelyAgree
                    = new LocalizationKey("answer.completely_agree") {fallback = "Totally!"};

                public static readonly LocalizationKey SomewhatAgree
                    = new LocalizationKey("answer.somewhat_agree") {fallback = "Kinda."};

                public static readonly LocalizationKey SomewhatDisagree
                    = new LocalizationKey("answer.somewhat_disagree") {fallback = "Kinda not."};

                public static readonly LocalizationKey CompletelyDisagree
                    = new LocalizationKey("answer.completely_disagree") {fallback = "No!"};

                public static readonly LocalizationKey SliderMuchMore
                    = new LocalizationKey("answer.slider_much_more") {fallback = "+++"};

                public static readonly LocalizationKey SliderMore
                    = new LocalizationKey("answer.slider_more") {fallback = "++"};

                public static readonly LocalizationKey SliderLittleMore
                    = new LocalizationKey("answer.slider_little_more") {fallback = "+"};

                public static readonly LocalizationKey SliderSame
                    = new LocalizationKey("answer.slider_same") {fallback = "="};

                public static readonly LocalizationKey SliderLittleLess
                    = new LocalizationKey("answer.slider_little_less") {fallback = "-"};

                public static readonly LocalizationKey SliderLess
                    = new LocalizationKey("answer.slider_less") {fallback = "--"};

                public static readonly LocalizationKey SliderMuchLess
                    = new LocalizationKey("answer.slider_much_less") {fallback = "---"};

                public static readonly LocalizationKey SliderAxisMin
                    = new LocalizationKey("answer.slider_axis_min") {fallback = "I disagree!"};

                public static readonly LocalizationKey SliderAxisMax
                    = new LocalizationKey("answer.slider_axis_max") {fallback = "I agree!"};

                public static readonly LocalizationKey Undecided
                    = new LocalizationKey("answer.undecided") {fallback = "I dunno."};
            }

            public static class Candidate
            {
                public static class Match
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("candidate.match.label") {fallback = "{value}% Match"};
                }

                public static class Party
                {
                    public static readonly LocalizationKey None
                        = new LocalizationKey("candidate.party.none") {fallback = "No Party"};

                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.party.value") {fallback = "{party}"};
                }

                public static class List
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.list.value") {fallback = "List {list}"};
                }

                public static class Place
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.place.value") {fallback = "({place})"};
                }

                public static class ListWithPlace
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.list_with_place.value") {fallback = "List {list} ({place})"};
                }

                public static class ListWithPlaces
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.list_with_places.value")
                            {fallback = "List {list} ({place}) +{count}"};
                }

                public static class ListPlace
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.list_place.value")
                            {fallback = "List place {number} {place}"};
                }

                public static class ListPlaces
                {
                    public static readonly LocalizationKey Value
                        = new LocalizationKey("candidate.list_places.value")
                            {fallback = "List place {number} ({place}) +{count}"};
                }

                public static readonly LocalizationKey Topics
                    = new LocalizationKey("candidate.topics") {fallback = "Topics"};

                public static readonly LocalizationKey Slogan
                    = new LocalizationKey("candidate.slogan") {fallback = "Slogan"};

                public static readonly LocalizationKey Education
                    = new LocalizationKey("candidate.education") {fallback = "Education"};

                public static readonly LocalizationKey Hobbies
                    = new LocalizationKey("candidate.hobbies") {fallback = "Hobbies"};

                public static readonly LocalizationKey CampaignBudget
                    = new LocalizationKey("candidate.campaign_budget") {fallback = "Campaign Budget"};

                public static readonly LocalizationKey TermsInOffice
                    = new LocalizationKey("candidate.terms_in_office") {fallback = "Terms In Office"};

                public static readonly LocalizationKey VestedInterests
                    = new LocalizationKey("candidate.vested_interests") {fallback = "Vested Interests"};

                public static readonly LocalizationKey NoProfileLabel
                    = new LocalizationKey("candidate.no_profile_label") {fallback = "Oh No!"};

                public static readonly LocalizationKey NoProfileDescription
                    = new LocalizationKey("candidate.no_profile_description")
                    {
                        fallback =
                            "{name} has not filled out any additional information on smartvote. You'll find more on them on the internet, in electoral brochures and other media."
                    };

                public static readonly LocalizationKey MandateYearCurrent
                    = new LocalizationKey("candidate.mandate_year_current") {fallback = "Today"};
            }

            public static class CandidateCollection
            {
                public static class Filter
                {
                    public static readonly LocalizationKey Name
                        = new LocalizationKey("candidate_collection.filter.name") {fallback = "Search"};

                    public static readonly LocalizationKey Party
                        = new LocalizationKey("candidate_collection.filter.party") {fallback = "Filter"};

                    public static readonly LocalizationKey PartyConfirm
                        = new LocalizationKey("candidate_collection.filter.party_confirm") {fallback = "Confirm"};
                }

                public static class Filters
                {
                    public static readonly LocalizationKey Confirm
                        = new LocalizationKey("candidate_collection.filters.confirm") {fallback = "Confirm"};

                    public static readonly LocalizationKey Label
                        = new LocalizationKey("candidate_collection.filters.label") {fallback = "Filter"};
                }

                public static class SortingOrder
                {
                    public static readonly LocalizationKey Age
                        = new LocalizationKey("candidate_collection.sorting_order.age") {fallback = "Age"};

                    public static readonly LocalizationKey Match
                        = new LocalizationKey("candidate_collection.sorting_order.match") {fallback = "Match"};

                    public static readonly LocalizationKey Name
                        = new LocalizationKey("candidate_collection.sorting_order.name") {fallback = "Name"};

                    public static readonly LocalizationKey Party
                        = new LocalizationKey("candidate_collection.sorting_order.party") {fallback = "Party"};
                }
            }

            public static class Cinematic
            {
                public static readonly LocalizationKey Skip
                    = new LocalizationKey("components.cinematic.skip") {fallback = "Skip"};
            }

            public static class ElectionList
            {
                public static class Age
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("components.election_list.age.label") {fallback = "Age"};
                }

                public static class ListNumber
                {
                    public static readonly LocalizationKey Official
                        = new LocalizationKey("election_list.number.official") {fallback = "List {number}"};

                    public static readonly LocalizationKey Modified
                        = new LocalizationKey("election_list.number.modified") {fallback = "List {number} (edited)"};

                    public static readonly LocalizationKey Custom
                        = new LocalizationKey("election_list.number.custom") {fallback = "Custom List"};
                }

                public static class Match
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("election_list.match.label") {fallback = "{value}% Total Match"};

                    public static readonly LocalizationKey Amount
                        = new LocalizationKey("election_list.match.amount") {fallback = "{value}%"};
                }

                public static class Name
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("election_list.name.label") {fallback = "Name"};

                    public static readonly LocalizationKey Placeholder
                        = new LocalizationKey("election_list.name.placeholder") {fallback = "Tap to enter name"};
                }

                public static class Seats
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("election_list.seats.label") {fallback = "Seats"};
                }
            }

            public static class Hints
            {
                public static readonly LocalizationKey TooFarWay
                    = new LocalizationKey("components.hints.too_far_away") {fallback = "You're too far away..."};

                public static readonly LocalizationKey MoreAccurateResults
                    = new LocalizationKey("components.hints.more_accurate_results")
                    {
                        fallback =
                            "The more challenges you complete, the more accurate your results will be. Hang in there!"
                    };

                public static readonly LocalizationKey Leaderboard
                    = new LocalizationKey("components.hints.leaderboard")
                    {
                        fallback = "Click on the LEADERBOARD to see your overall winners."
                    };

                public static readonly LocalizationKey LikeAndDislike
                    = new LocalizationKey("components.hints.like_and_dislike")
                    {
                        fallback = "You can like or dislike candidates to give them a special marking."
                    };

                public static readonly LocalizationKey LeaderboardFilter
                    = new LocalizationKey("components.hints.leaderboard_filter")
                    {
                        fallback = "You can filter the leaderboard to see specific parties."
                    };
            }

            public static class Race
            {
                public static readonly LocalizationKey CategoryMatch
                    = new LocalizationKey("components.race.category_match") {fallback = "Category Match"};
            }
        }

        public static class Notifications
        {
            public static class UpcomingElection
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("notifications.upcoming_election.title") {fallback = "Upcoming Election!"};

                public static readonly LocalizationKey Text
                    = new LocalizationKey("notifications.upcoming_election.text")
                    {
                        fallback =
                            "{election} will be held on the {day}.{month}.{year}! Are you ready to cast your vote?"
                    };
            }
        }

        public static class Views
        {
            public static class Candidate
            {
                public static readonly LocalizationKey Compare
                    = new LocalizationKey("views.candidate.compare") {fallback = "Compare"};

                public static readonly LocalizationKey ReadMore
                    = new LocalizationKey("views.candidate.read_more") {fallback = "Read More"};

                public static readonly LocalizationKey ToggleCandidate
                    = new LocalizationKey("views.candidate.toggle_candidate") {fallback = "Candidate"};

                public static readonly LocalizationKey TogglePlayer
                    = new LocalizationKey("views.candidate.toggle_player") {fallback = "You"};

                public static readonly LocalizationKey TogglePlayerPrevious
                    = new LocalizationKey("views.candidate.toggle_player") {fallback = "Previous"};
            }

            public static class CandidateAnswers
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.candidate_answers.title") {fallback = "smartvote Questionnaire"};

                public static readonly LocalizationKey CategoryTitle
                    = new LocalizationKey("views.candidate_answers.category_title") {fallback = "{index}. {category}"};

                public static readonly LocalizationKey CategoryProgressValue
                    = new LocalizationKey("views.candidate_answers.category_progress_value")
                        {fallback = "{answered}/{questions}"};

                public static readonly LocalizationKey CategoryMatchValue
                    = new LocalizationKey("views.candidate_answers.category_match_value") {fallback = "{match}%"};

                public static readonly LocalizationKey CategoryMatchLabel
                    = new LocalizationKey("views.candidate_answers.category_match_label") {fallback = "Category Match"};

                public static readonly LocalizationKey Question
                    = new LocalizationKey("views.candidate_answers.question") {fallback = "{index}. {question}"};
            }

            public static class CandidateProfile
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.candidate_profile.title") {fallback = "Profile"};
            }

            public static class CandidateResult
            {
                public static readonly LocalizationKey Header
                    = new LocalizationKey("views.candidate_result.header") {fallback = "You discovered:"};

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.candidate_result.confirm") {fallback = "Continue"};

                public static readonly LocalizationKey Match
                    = new LocalizationKey("views.candidate_result.match", "{value}")
                        {fallback = "{value}% Total Match"};
            }

            public static class Celebration
            {
                public static readonly LocalizationKey CategoryComplete
                    = new LocalizationKey("views.celebration.category_complete") {fallback = "Category Complete!"};
            }

            public static class Congratulations
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.congratulation.category_complete") {fallback = "CONGRATULATIONS!"};

                public static readonly LocalizationKey RaceRoyaleComplete
                    = new LocalizationKey("views.congratulation.race_royale_complete")
                        {fallback = "Race Royale Complete!"};

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.congratulation.confirm") {fallback = "Hooray!"};
            }

            public static class Credits
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.credits.title")
                    {
                        fallback = "Credits | Project CH+"
                    };
            }

            public static class EndingCredits
            {
                public static readonly LocalizationKey Continue
                    = new LocalizationKey("views.ending_credits.continue") {fallback = "Continue"};
            }

            public static class ExtraInfo
            {
                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.extra_info.confirm") {fallback = "Got it!"};
            }

            public static class FaceSelection
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.face_selection.title")
                    {
                        fallback = "Choose your look."
                    };

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.face_selection.confirm")
                    {
                        fallback = "Something like this."
                    };
            }

            public static class Final
            {
                public static readonly LocalizationKey Leaderboard
                    = new LocalizationKey("views.final.leaderboard") {fallback = "Leaderboard"};

                public static readonly LocalizationKey Donate
                    = new LocalizationKey("views.final.donate") {fallback = "Donate"};
            }

            public static class HowToPlay
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.how_to_play.title")
                    {
                        fallback = "How to Play"
                    };
            }

            public static class HowToVote
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.how_to_vote.title")
                    {
                        fallback = "How to Vote"
                    };
            }

            public static class Informations
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.informations.title")
                    {
                        fallback = "About Dope Elections"
                    };
            }

            public static class Leaderboard
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.leaderboard.title") {fallback = "Race Royale Leaderboard"};

                public static readonly LocalizationKey Instruction
                    = new LocalizationKey("views.leaderboard.instruction")
                        {fallback = "These are your overall champions."};

                public static readonly LocalizationKey Index
                    = new LocalizationKey("views.leaderboard.index") {fallback = "{min}-{max}"};

                public static readonly LocalizationKey AllCouncils
                    = new LocalizationKey("views.leaderboard.all_councils") {fallback = "All Candidates"};

                public static readonly LocalizationKey Filter
                    = new LocalizationKey("views.leaderboard.filter") {fallback = "Filter"};
            }

            public static class LeaderboardFilter
            {
                public static readonly LocalizationKey Order
                    = new LocalizationKey("views.leaderboard_filter.title") {fallback = "Order"};

                public static readonly LocalizationKey Match
                    = new LocalizationKey("views.leaderboard_filter.match") {fallback = "Match"};

                public static readonly LocalizationKey Name
                    = new LocalizationKey("views.leaderboard_filter.name") {fallback = "Name"};

                public static readonly LocalizationKey Party
                    = new LocalizationKey("views.leaderboard_filter.party") {fallback = "Party"};

                public static readonly LocalizationKey Age
                    = new LocalizationKey("views.leaderboard_filter.age") {fallback = "Age"};

                public static readonly LocalizationKey PartyFilter
                    = new LocalizationKey("views.leaderboard_filter.party_filter") {fallback = "Party Filter"};

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.leaderboard_filter.confirm") {fallback = "Confirm"};
            }

            public static class LocationSelection
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.location_selection.title")
                    {
                        fallback = "Where are you?"
                    };

                public static readonly LocalizationKey Zip
                    = new LocalizationKey("views.location_selection.zip")
                    {
                        fallback = "Postal Code"
                    };

                public static readonly LocalizationKey Canton
                    = new LocalizationKey("views.location_selection.canton")
                    {
                        fallback = "Canton"
                    };

                public static readonly LocalizationKey Constituency
                    = new LocalizationKey("views.location_selection.constituency")
                    {
                        fallback = "Voting District"
                    };

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.location_selection.confirm")
                    {
                        fallback = "Here!"
                    };

                public static readonly LocalizationKey Or
                    = new LocalizationKey("views.location_selection.or")
                    {
                        fallback = "or"
                    };

                public static readonly LocalizationKey Instruction
                    = new LocalizationKey("views.location_selection.instruction")
                    {
                        fallback =
                            "Select your location."
                    };

                public static readonly LocalizationKey YouVoteHere
                    = new LocalizationKey("views.location_selection.you_vote_here")
                    {
                        fallback = "You vote in <b>{constituency}</b>."
                    };

                public static readonly LocalizationKey ZipNotFound
                    = new LocalizationKey("views.location_selection.zip_not_found")
                    {
                        fallback =
                            "We can't find your location.\nPlease check your postal code or try selecting your voting district directly.\n\nAt the moment, this application works exclusively in the canton of Neuchatel."
                    };
            }

            public static class OurSystem
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.our_system.title")
                    {
                        fallback = "Our System"
                    };
            }

            public static class Overview
            {
                public static readonly LocalizationKey Race
                    = new LocalizationKey("views.overview.race") {fallback = "Race"};

                public static readonly LocalizationKey Vote
                    = new LocalizationKey("views.overview.vote") {fallback = "Vote"};

                public static class FirstRaceTutorial
                {
                    public static readonly LocalizationKey FirstLine
                        = new LocalizationKey("views.overview.first_race_tutorial.first_line")
                            {fallback = "May the Race begin!"};

                    public static readonly LocalizationKey SecondLine
                        = new LocalizationKey("views.overview.first_race_tutorial.second_line")
                            {fallback = "Find out who is up to"};

                    public static readonly LocalizationKey ThirdLine
                        = new LocalizationKey("views.overview.first_race_tutorial.third_line")
                            {fallback = "Your standards!"};
                }
            }

            public static class Progression
            {
                public static readonly LocalizationKey Leaderboard
                    = new LocalizationKey("views.progression.leaderboard") {fallback = "Leaderboard"};

                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.progression.title") {fallback = "Race Royale"};

                public static class LockedEntry
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.progression.locked_entry.label") {fallback = "Locked"};
                }

                public static class PickCategoryEntry
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.progression.pick_category_entry.label")
                            {fallback = "Pick a Category"};

                    public static readonly LocalizationKey HelpText
                        = new LocalizationKey("views.progression.pick_category_entry.unlocked_help_text")
                        {
                            fallback = "Complete levels to find suitable candidates!"
                        };
                }

                public static class UnknownCategoryEntry
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.progression.unknown_category_entry.label")
                            {fallback = "Unknown Category"};
                }

                public static class TeamRace
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.progression.team_race.label") {fallback = "Team Race"};

                    public static readonly LocalizationKey UnlockedHelpText
                        = new LocalizationKey("views.progression.team_race.unlocked_help_text")
                        {
                            fallback = "Let's race your team against the rest of the pack!"
                        };

                    public static readonly LocalizationKey LockedHelpText
                        = new LocalizationKey("views.progression.team_race.locked_help_text")
                        {
                            fallback = "<b>Team Race locked:</b> Tap here to start creating your team!"
                        };
                }
            }

            public static class QuestionInfo
            {
                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.question_info.confirm") {fallback = "OK"};
            }

            public static class RaceInfo
            {
                public static readonly LocalizationKey Confirm = new LocalizationKey("views.race_info.confirm")
                {
                    fallback = "OK"
                };
            }

            public static class RaceReview
            {
                public static readonly LocalizationKey ChallengeComplete
                    = new LocalizationKey("views.race_review.challenge_complete") {fallback = "Challenge Complete!"};

                public static readonly LocalizationKey SmartSpider
                    = new LocalizationKey("views.race_review.smart_spider") {fallback = "smartspider"};

                public static readonly LocalizationKey TopChampions
                    = new LocalizationKey("views.race_review.top_champions") {fallback = "Your Top Champions"};

                public static readonly LocalizationKey HugeTie
                    = new LocalizationKey("views.race_review.huge_tie") {fallback = "It was a huge tie!"};

                public static readonly LocalizationKey HugeTieText
                    = new LocalizationKey("views.race_review.huge_tie_text")
                    {
                        fallback = "There were too many candidates with the same score. Continue the Race Royale or " +
                                   "repeat this challenge to determine who your clear winners are!"
                    };

                public static readonly LocalizationKey CategoryMatch
                    = new LocalizationKey("views.race_review.category_match") {fallback = "{value}% Category Match"};

                public static readonly LocalizationKey Continue
                    = new LocalizationKey("views.race_review.continue") {fallback = "Continue"};

                public static readonly LocalizationKey Repeat
                    = new LocalizationKey("views.race_review.repeat") {fallback = "Repeat"};
            }

            public static class RandomCategorySelection
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.category.title") {fallback = "Choose Your Challenge To Start!"};

                public static readonly LocalizationKey ChallengeOverview
                    = new LocalizationKey("views.category.challenge_overview") {fallback = "Challenge Overview"};

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.category.confirm") {fallback = "Go!"};

                public static readonly LocalizationKey InstructionChoose
                    = new LocalizationKey("views.category.instruction_choose")
                        {fallback = "Choose a challenge, reshuffle or choose from the overview."};

                public static readonly LocalizationKey InstructionConfirm
                    = new LocalizationKey("views.category.instruction_confirm")
                    {
                        fallback =
                            "This challenge has <b>{questions}</b> questions. <b>{answered}/{questions}</b> answered. <b>Ready?</b>"
                    };
            }

            public static class Scanner
            {
                public static class NoCameraAlert
                {
                    public static readonly LocalizationKey Title
                        = new LocalizationKey("views.scanner.no_camera.title")
                        {
                            fallback = "Camera required!"
                        };

                    public static readonly LocalizationKey Text
                        = new LocalizationKey("views.scanner.no_camera.text")
                        {
                            fallback =
                                "Please make sure you to enable camera access in your device settings to use the scan functionality."
                        };

                    public static readonly LocalizationKey Confirm
                        = new LocalizationKey("views.scanner.no_camera.confirm") {fallback = "Continue"};
                }

                public static class NoMatchAlert
                {
                    public static readonly LocalizationKey Text
                        = new LocalizationKey("views.scanner.no_match.text")
                        {
                            fallback =
                                "Nothing found! Please make sure to focus the poster and remove any obstructions."
                        };

                    public static readonly LocalizationKey Confirm
                        = new LocalizationKey("views.scanner.no_match.confirm") {fallback = "Continue"};
                }

                public static readonly LocalizationKey Scan
                    = new LocalizationKey("views.scanner.scan") {fallback = "Scan!"};
            }

            public static class Settings
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.settings.title") {fallback = "Settings"};

                public static readonly LocalizationKey Language
                    = new LocalizationKey("views.settings.language") {fallback = "Language"};

                public static readonly LocalizationKey MasterVolume
                    = new LocalizationKey("views.settings.master_volume") {fallback = "Master"};

                public static readonly LocalizationKey MusicVolume
                    = new LocalizationKey("views.settings.music_volume") {fallback = "Music"};

                public static readonly LocalizationKey UiVolume
                    = new LocalizationKey("views.settings.ui_volume") {fallback = "Menu"};

                public static readonly LocalizationKey EffectVolume
                    = new LocalizationKey("views.settings.effect_volume") {fallback = "Effects"};

                public static readonly LocalizationKey AmbienceVolume
                    = new LocalizationKey("views.settings.ambient_volume") {fallback = "Ambience"};

                public static readonly LocalizationKey DeleteDownloadedData
                    = new LocalizationKey("views.settings.delete_downloaded_data") {fallback = "Delete Downloads"};

                public static readonly LocalizationKey DeleteAllData
                    = new LocalizationKey("views.settings.delete_local_data") {fallback = "Delete All Data"};

                public static readonly LocalizationKey Instruction
                    = new LocalizationKey("views.settings.instruction")
                        {fallback = "You did this! Do you have to change it?"};

                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.settings.confirm") {fallback = "Save"};

                public static class DeleteDownloadedDataPrompt
                {
                    public static readonly LocalizationKey Title
                        = new LocalizationKey("views.settings.delete_downloaded_data.title")
                            {fallback = "Are you sure?"};

                    public static readonly LocalizationKey Description
                        = new LocalizationKey("views.settings.delete_downloaded_data.description")
                        {
                            fallback =
                                "This action cannot be undone. Deletes all downloaded data. After completion, the game will restart."
                        };

                    public static readonly LocalizationKey Confirm
                        = new LocalizationKey("views.settings.delete_downloaded_data.confirm")
                            {fallback = "Delete Data"};

                    public static readonly LocalizationKey Cancel
                        = new LocalizationKey("views.settings.delete_local_data.cancel") {fallback = "Cancel"};
                }

                public static class DeleteAllDataPrompt
                {
                    public static readonly LocalizationKey Title
                        = new LocalizationKey("views.settings.delete_local_data.title") {fallback = "Are you sure?"};

                    public static readonly LocalizationKey Description
                        = new LocalizationKey("views.settings.delete_local_data.description")
                        {
                            fallback =
                                "This action cannot be undone. Deletes all downloaded data, settings and progress. After completion, the game will restart."
                        };

                    public static readonly LocalizationKey Confirm
                        = new LocalizationKey("views.settings.delete_local_data.confirm") {fallback = "Delete Data"};

                    public static readonly LocalizationKey Cancel
                        = new LocalizationKey("views.settings.delete_local_data.cancel") {fallback = "Cancel"};
                }
            }

            public static class SmartSpiderInfo
            {
                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.smart_spider_info.confirm")
                    {
                        fallback = "Ok"
                    };
            }

            public static class Splash
            {
                public static readonly LocalizationKey Start
                    = new LocalizationKey("views.splash.start") {fallback = "Start Race Royale"};
            }

            public static class Startup
            {
                public static readonly LocalizationKey Connecting
                    = new LocalizationKey("views.startup.connecting") {fallback = "Connecting..."};

                public static readonly LocalizationKey LoadLocalFiles
                    = new LocalizationKey("views.startup.load_local_files") {fallback = "Loading local data..."};

                public static readonly LocalizationKey Downloading
                    = new LocalizationKey("views.startup.downloading") {fallback = "Downloading..."};

                public static class DownloadFailedAlert
                {
                    public static readonly LocalizationKey Title
                        = new LocalizationKey("views.download_failed_alert.title") {fallback = "Oh no!"};

                    public static readonly LocalizationKey Text
                        = new LocalizationKey("views.download_failed_alert.text")
                        {
                            fallback =
                                "Something went wrong while downloading data. Please check your internet connection and try again."
                        };
                }

                public static class TranslationIncompleteAlert
                {
                    public static readonly LocalizationKey Title
                        = new LocalizationKey("views.translation_incomplete_alert.title") {fallback = "Oh no!"};

                    public static readonly LocalizationKey Text
                        = new LocalizationKey("views.translation_incomplete_alert.text")
                        {
                            fallback =
                                "The current election seems to have incomplete translations in your language. Please select a different language in the settings."
                        };

                    public static readonly LocalizationKey ChangeRegion
                        = new LocalizationKey("views.translation_incomplete_alert.change_region")
                        {
                            fallback = "Change region"
                        };

                    public static readonly LocalizationKey ChangeLanguage
                        = new LocalizationKey("views.translation_incomplete_alert.change_language")
                        {
                            fallback = "Change language"
                        };
                }
            }

            public static class TeamMenu
            {
                public static class CustomListsTab
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.custom_lists_tab.label")
                            {fallback = "Custom"};
                }

                public static class DumpZone
                {
                    public static readonly LocalizationKey Text
                        = new LocalizationKey("views.team.dump_zone.text")
                            {fallback = "Drop {name} here to exclude them from your team."};
                }

                public static class EditCandidates
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.edit_candidates.label")
                            {fallback = "Edit Candidates"};
                }

                public static class Export
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.export.label") {fallback = "Save"};
                }

                public static class Import
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.import.label") {fallback = "Choose"};
                }

                public static class Info
                {
                    public static readonly LocalizationKey TeamLimit
                        = new LocalizationKey("views.team.info.team_limit") {fallback = "{amount}/{limit}"};

                    public static readonly LocalizationKey RepetitionsLimit
                        = new LocalizationKey("views.team.info.repetitions_limit")
                            {fallback = "{name} is already {limit} times on your list."};
                }

                public static class OfficialListsTab
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.official_lists_tab.label")
                            {fallback = "Official"};
                }

                public static class ShowPrintVersion
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.show_print_version.label")
                            {fallback = "Print"};
                }

                public static class StartPhotoShoot
                {
                    public static readonly LocalizationKey Label
                        = new LocalizationKey("views.team.start_photo_shoot.label")
                            {fallback = "Photo Shoot"};
                }

                public static class Tutorial
                {
                    public static class Export
                    {
                        public static readonly LocalizationKey Title
                            = new LocalizationKey("views.team.tutorial_export.title") {fallback = "Hint"};

                        public static readonly LocalizationKey Text
                            = new LocalizationKey("views.team.tutorial_export.text")
                            {
                                fallback = "Pick a candidate list from above to overwrite or save as a new entry."
                            };
                    }

                    public static class Import
                    {
                        public static readonly LocalizationKey Title
                            = new LocalizationKey("views.team.tutorial_import.title") {fallback = "Hint"};

                        public static readonly LocalizationKey Text
                            = new LocalizationKey("views.team.tutorial_import.text")
                            {
                                fallback = "Pick a candidate list from above as a starting point for your own list."
                            };
                    }
                }
            }

            public static class UserConsent
            {
                public static readonly LocalizationKey Confirm
                    = new LocalizationKey("views.candidate_answers.confirm") {fallback = "Accept"};

                public static readonly LocalizationKey Reject
                    = new LocalizationKey("views.candidate_answers.reject") {fallback = "Reject"};
            }

            public static class VoteInfos
            {
                public static readonly LocalizationKey Title
                    = new LocalizationKey("views.vote.title")
                    {
                        fallback = "Vote"
                    };
            }
        }
    }
}