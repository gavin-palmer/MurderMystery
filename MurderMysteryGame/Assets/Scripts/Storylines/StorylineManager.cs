using System;
using System.Collections.Generic;
using System.Linq;
using MurderMystery.Interfaces;
using MurderMystery.Storylines;

namespace MurderMystery.Generators
{
    public class StorylineManager
    {
        private List<IStorylineStrategy> _availableStorylines = new List<IStorylineStrategy>();
        private Random _random;

        public StorylineManager(Random random)
        {
            _random = random;
            RegisterDefaultStorylines();
        }

        private void RegisterDefaultStorylines()
        {
            RegisterStoryline(new AffairStoryline());
            RegisterStoryline(new SecretDiscoveryStoryline());
            RegisterStoryline(new TheftStoryline());
            RegisterStoryline(new DrunkennessStoryline());
            RegisterStoryline(new MysteriousPhoneCallStoryline());
        }

        public void RegisterStoryline(IStorylineStrategy storyline)
        {
            _availableStorylines.Add(storyline);
        }

        public List<IStorylineStrategy> SelectRandomStorylines(int count)
        {
            count = Math.Min(count, _availableStorylines.Count);
            return _availableStorylines
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();
        }

        public void ExecuteStorylines(TimelineContext context, int count)
        {
            var selectedStorylines = SelectRandomStorylines(count);
            foreach (var storyline in selectedStorylines)
            {
                try
                {
                    storyline.Execute(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing storyline {storyline.Name}: {ex.Message}");
                }
            }
        }
    }
}