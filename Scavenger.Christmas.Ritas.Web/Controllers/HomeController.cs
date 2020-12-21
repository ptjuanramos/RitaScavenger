using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scavenger.Christmas.Ritas.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scavenger.Christmas.Ritas.Web
{
    public class HomeController : Controller
    {
        private const string CurrentRiddleKey = "CurrentRiddle";
        private const string HasFinishedRiddleKey = "CurrentRiddle";

        private readonly IDictionary<int, RiddleAnswer> _riddlesAndAnswers = new Dictionary<int, RiddleAnswer>()
        {
            [1] = new SingleRiddleAnswer
            {
                Answer = "value1", //TODO
                NextRiddleQuestion = "Test" //TODO
            },
            [2] = new MultipleRiddleAnswer
            {
                Answers =  new Dictionary<int, string>()
                {
                    [0] = "sesimbra",
                    [1] = "bacalhau",
                    [2] = "polo-norte",
                    [3] = "fabio-porchat",
                    [4] = "belchior",
                    [5] = "um"
                }
            }
        };

        private MultipleRiddleAnswer GetOnlyQuestions(MultipleRiddleAnswer multipleRiddleAnswer)
        {
            Dictionary<int, string> dictionaryWithoutAnswers = multipleRiddleAnswer.Answers.Keys
                .GroupBy(k => k)
                .ToDictionary(kv => kv.Key, kv => string.Empty);

            return new MultipleRiddleAnswer
            {
                Answers = dictionaryWithoutAnswers
            };
        }

        public HomeController()
        {
        }

        private int GetRiddleNumber()
        {
            int? sessionRiddleNumber = HttpContext.Session.GetInt32(CurrentRiddleKey);
            return sessionRiddleNumber ?? 1;
        }

        public IActionResult Index()
        {
            if (HasFinished())
                return RedirectToAction(nameof(End));

            TempData["RiddleNumber"] = GetRiddleNumber();
            int riddleNumber = (TempData["RiddleNumber"] as int?).GetValueOrDefault();

            bool? isMultipleAnswer = TempData["IsMultipleAnswer"] as bool?;

            if (isMultipleAnswer.GetValueOrDefault())
            {
                return View(new Riddle
                {
                    MultipleRiddleAnswer = GetOnlyQuestions(_riddlesAndAnswers[riddleNumber] as MultipleRiddleAnswer)
                });
            }

            return View();
        }

        private IActionResult NextRiddle(int currentRiddleNumber,string nextRiddleQuestion)
        {
            int nextRiddleNumber = currentRiddleNumber + 1;

            if(_riddlesAndAnswers.ContainsKey(nextRiddleNumber))
            {
                RiddleAnswer answer = _riddlesAndAnswers[nextRiddleNumber];
                TempData["IsMultipleAnswer"] = answer is MultipleRiddleAnswer;

                HttpContext.Session.SetInt32(CurrentRiddleKey, nextRiddleNumber);
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetInt32(HasFinishedRiddleKey, 1);
            return RedirectToAction(nameof(Index));
        }

        private RiddleAnswer GetRiddleAnswer(int currentRiddleNumber)
        {
            if(_riddlesAndAnswers.TryGetValue(currentRiddleNumber, out RiddleAnswer value))
            {
                return value;
            }

            return null;
        }

        public IActionResult End()
        {
            TempData["HasFinished"] = true;
            return View();
        }

        private bool HasFinished()
        {
            int? hasFinishedHasInt = HttpContext.Session.GetInt32(HasFinishedRiddleKey);
            return hasFinishedHasInt.GetValueOrDefault() == 1;
        }

        [HttpPost]
        public IActionResult Answer(Riddle riddle)
        {
            if (riddle.MultipleRiddleAnswer == null)
                return Answer(riddle.SingleRiddleAnswer);

            return Answer(riddle.MultipleRiddleAnswer);
        }

        private IActionResult RedirectToQuestionWithWrongAnswer()
        {
            TempData["ErrorMessage"] = "Wrong answer...";
            return RedirectToAction(nameof(Index));
        }

        private IActionResult Answer(SingleRiddleAnswer riddle)
        {
            int currentRiddleNumber = GetRiddleNumber();
            SingleRiddleAnswer riddleAnswer = GetRiddleAnswer(currentRiddleNumber) as SingleRiddleAnswer;

            if (riddleAnswer != null && riddle.Answer == riddleAnswer.Answer)
            {
                TempData["HasSucceeded"] = true;
                return NextRiddle(currentRiddleNumber, riddleAnswer.NextRiddleQuestion);
            }

            return RedirectToQuestionWithWrongAnswer();
        }

        private IActionResult Answer(MultipleRiddleAnswer riddle)
        {
            int currentRiddleNumber = GetRiddleNumber();
            MultipleRiddleAnswer riddleAnswer = GetRiddleAnswer(currentRiddleNumber) as MultipleRiddleAnswer;

            if (IsMultipleRiddleCorrect(riddleAnswer, riddle))
            {
                TempData["HasSucceeded"] = true;
                return NextRiddle(currentRiddleNumber, riddleAnswer.NextRiddleQuestion);
            }

            TempData["IsMultipleAnswer"] = true;

            return RedirectToQuestionWithWrongAnswer();
        }

        private bool EqualWithoutSlash(string str1, string str2)
        {
            char slash = '-';
            char space = ' ';
            char empty = '\0';

            string str1WithoutSlash = str1.Replace(slash, empty).Replace(space, empty);
            string str2WithoutSlash = str2.Replace(slash, empty).Replace(space, empty);

            return str1WithoutSlash == str2WithoutSlash;
        }

        private bool IsMultipleRiddleCorrect(MultipleRiddleAnswer riddle, MultipleRiddleAnswer answer)
        {
            foreach(KeyValuePair<int, string> kv in riddle.Answers)
            {
                string questionAnswer = answer.Answers[kv.Key];
                if(!EqualWithoutSlash(questionAnswer.ToUpper(), kv.Value.ToUpper()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
