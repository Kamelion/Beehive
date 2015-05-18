using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Ch13exer4AdvancedBeehiveOverhaul
{
    [Serializable]
    public class Hive
    {
        public const int InitialBees = 6;
        public const double InitialHoney = 3.2;
        public const double MaximumHoney = 15;
        public const double NectarHoneyRatio = 0.25;
        public const int MaximumBees = 8;
        public const double MinimumHoneyForCreatingBees = 4;
        public double Honey { get; private set; }
        private Dictionary<string, Point> locations;
        private int beeCount = 0;
        private World World;
        
        [NonSerialized]
        public BeeMessage MessageSender;

        public Hive(World World, BeeMessage MessageSender)
        {
            this.World = World;
            this.MessageSender = MessageSender;
            Honey = InitialHoney;
            InitializeLocations();
            Random random = new Random();
            for (int i = 0; i < InitialBees; i++)
            {
                AddBee(random);
            }
        }

        public Point GetLocation(string location)
        {
            if (locations.ContainsKey(location))
            {
                return locations[location];
            } else 
                throw new ArgumentException("Unknown location: " + location);            
        }

        public void InitializeLocations()
        {
            locations = new Dictionary<string, Point>();
            locations.Add("Entrance", new Point(600, 100));
            locations.Add("Nursery", new Point(95, 182));
            locations.Add("HoneyFactory", new Point(187, 93));
            locations.Add("Exit", new Point(212, 222));
        }

        public bool AddHoney(double nectar)
        {
            double honeyToAdd = nectar * NectarHoneyRatio;
            if (honeyToAdd + Honey > MaximumHoney)
                return false;
            Honey += honeyToAdd;
            return true;
        }

        public bool ConsumeHoney(double amount)
        {
            if (amount > Honey)
                return false;
            else
            {
                Honey -= amount;
                return true;
            }
        }

        private void AddBee(Random random)
        {
            beeCount++;
            int r1 = random.Next(100) - 50;
            int r2 = random.Next(100) - 50;
            Point startPoint = new Point(locations["Nursery"].X + r1,
                                         locations["Nursery"].Y + r2);
            Bee newBee = new Bee(beeCount, startPoint, this, World);
            newBee.MessageSender += this.MessageSender;
            World.Bees.Add(newBee);
            // Once we have a system, we need to add this bee to the system
        }

        public void Go(Random random)
        {
            if (World.Bees.Count < MaximumBees &&
                Honey > MinimumHoneyForCreatingBees &&
                random.Next(10) == 1)
                AddBee(random);
        }
    }
}
