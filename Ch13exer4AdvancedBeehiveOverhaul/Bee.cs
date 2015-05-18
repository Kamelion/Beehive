using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Ch13exer4AdvancedBeehiveOverhaul
{
    [Serializable]
    public class Bee
    {
        private const double HoneyConsumed = 0.5;
        private const int MoveRate = 3;
        private const double MinimumFlowerNectar = 1.5;
        private const int CareerSpan = 1000;

        public int Age { get; private set; }
        public bool InsideHive { get; private set; }
        public double NectarCollected { get; private set; }
        public BeeState CurrentState { get; private set; }
        private Hive Hive;
        private World World;

        [NonSerialized]
        public BeeMessage MessageSender;

        private Point location;
        public Point Location
        {
            get { return location; }
        }

        private int ID;
        private Flower destinationFlower;

        public Bee(int id, Point location, Hive Hive, World World)
        {
            this.Hive = Hive;
            this.World = World;
            this.ID = id;
            Age = 0;
            this.location = location;
            InsideHive = true;
            destinationFlower = null;
            NectarCollected = 0;
            CurrentState = BeeState.Idle;
        }

        public void Go(Random random)
        {
            Age++;
            BeeState oldState = CurrentState;
            switch (CurrentState)
            {
                case BeeState.Idle:
                    if (Age > CareerSpan)
                    {
                        CurrentState = BeeState.Retired;
                    }
                    else if (World.Flowers.Count > 0
                            && Hive.ConsumeHoney(HoneyConsumed))
                    {
                        Flower flower = World.Flowers[random.Next(World.Flowers.Count)];
                        if (flower.Nectar >= MinimumFlowerNectar && flower.Alive)
                        {
                            destinationFlower = flower;
                            CurrentState = BeeState.FlyingToFlower;
                        }
                    }
                    break;
                case BeeState.FlyingToFlower:
                    // Change point where bees gather nectar from
                    Point offsetFlowerLocation = new Point(destinationFlower.Location.X + 30,
                                                           destinationFlower.Location.Y + 20);
                    if (!World.Flowers.Contains(destinationFlower))
                        CurrentState = BeeState.ReturningToHive;
                    else if (InsideHive)
                    {
                        if (MoveTowardsLocation(Hive.GetLocation("Exit")))
                        {
                            InsideHive = false;
                            location = Hive.GetLocation("Entrance");
                        }
                    }
                    else if (MoveTowardsLocation(offsetFlowerLocation))
                            CurrentState = BeeState.GatheringNectar;
                    break;
                case BeeState.GatheringNectar:
                    double nectar = destinationFlower.HarvestNectar();
                    if (nectar > 0)
                        NectarCollected += nectar;
                    else
                        CurrentState = BeeState.ReturningToHive;
                    break;
                case BeeState.ReturningToHive:
                    if (!InsideHive)
                    {
                        if (MoveTowardsLocation(Hive.GetLocation("Entrance")))
                        {
                            InsideHive = true;
                            location = Hive.GetLocation("Exit");
                        }
                    }
                    else
                    {
                        if (MoveTowardsLocation(Hive.GetLocation("HoneyFactory")))
                            CurrentState = BeeState.MakingHoney;
                    }
                    break;
                case BeeState.MakingHoney:
                    if (NectarCollected < 0.5)
                    {
                        NectarCollected = 0;
                        CurrentState = BeeState.Idle;
                    }
                    else if (Hive.AddHoney(0.5))
                        NectarCollected -= 0.5;
                    else
                        NectarCollected = 0;
                    break;
                case BeeState.Retired:
                    // Do nothing! We're retired!
                    break;
            }
            if (oldState != CurrentState && MessageSender != null)
                MessageSender(ID, CurrentState.ToString());

        }

        private bool MoveTowardsLocation(Point destination)
        {
            if (Math.Abs(destination.X - location.X) <= MoveRate &&
                Math.Abs(destination.Y - location.Y) <= MoveRate)
                return true;

            if (destination.X > location.X)
                location.X += MoveRate;
            else if (destination.X < location.X)
                location.X -= MoveRate;

            if (destination.Y > location.Y)
                location.Y += MoveRate;
            else if (destination.Y < location.Y)
                location.Y -= MoveRate;

            return false;
        }
    }
}
