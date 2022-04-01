using System;
using System.Collections.Generic;
using System.Threading;

namespace Observer
{
    public class Property
    {
        public string Address { get; set; }
        public int Price { get; set; }
        public IObserver Owner { get; set; }

        public Property(string A)
        {
            this.Address = A;
            this.Price = 500000;
            this.Owner = null;
        }
    }

    public interface IObserver
    {
        string Name { get; set; }
        public void Update(object o);
        //public bool MakeOffer();
    }

    public class Observer : IObserver
    {
        public string Name { get; set; }
        public int MaxLimit { get; set; }
        public int MyOffer { get; set; }
        private Property p;
        ISaleble Auction;
        IObservable AgentObs;

        public Observer(string name, int maxLimit, ISaleble a)
        {
            Name = name;
            MaxLimit = maxLimit;
            Auction = a;
            AgentObs = (Observable)Auction;
            AgentObs.Subsribe(this);
            p = Auction.property;
        }

        public void Update(object o)
        {
            p = (Property)o;

        }

        public bool MakeOffer()
        {
            if (p.Price + 5000 < this.MaxLimit)
            {
                this.MyOffer = p.Price + 5000;
                if (!Auction.BidUp(this, MyOffer))
                {
                    return false;
                }
                return true;

            }
            else
                return false;
        }
    }

    public interface ISaleble
    {
        public bool BidUp(IObserver o, int price);
        public Property property { get; set; }
        public List<IObserver> observers { get; set; }
        public void Congratulations();
    }

    public interface IObservable
    {
        public void Subsribe(IObserver o);
        public void Unsubscribe(IObserver o);
        public void Notify();
    }

    public class Observable : IObservable, ISaleble
    {
        public Property property { get; set; }
        public List<IObserver> observers { get; set; }

        public Observable(string address)
        {
            observers = new List<IObserver>();
            property = new Property(address);// { Address = address };
        }

        public void Subsribe(IObserver o)
        {
            observers.Add(o);
        }

        public void Unsubscribe(IObserver o)
        {
            observers.Remove(o);
        }

        public void Notify()
        {
            foreach (IObserver obs in observers)
            {
                obs.Update(property);
            }
            if (observers.Count == 1)
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("Sold  to {0}, for price {1}/ Congratulations, {0}!", property.Owner.Name, property.Price);
                return;
            }
        }

        public void Congratulations()
        {
            if (!(property.Owner == null))
                Console.WriteLine("{0} sold by {1} for price - {2}", property.Address, property.Owner.Name, property.Price);
            else Console.WriteLine("Nobody no sale property " + property.Address);
        }

        public bool BidUp(IObserver o, int price)
        {
            if (price > property.Price)
            {
                property.Price = price;
                property.Owner = o;
                Notify();
                Console.WriteLine("New offer from {0} with price {1}", o.Name, price);
                return true;
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IObservable GenaAgent = new Observable("7 Palermo street");
            ISaleble auction = (Observable)GenaAgent;
            Observer o1 = new Observer("Dima", 600000, auction);
            Observer o2 = new Observer("Roma", 621000, auction);
            Observer o3 = new Observer("Petr", 621000, auction);
            List<Observer> observers = new List<Observer> { o1, o2, o3 };
            int i = 0;
            while (observers.Count > 1)
            {
                Console.WriteLine("{0}, would you like to make offer?  Press Enter", observers[i].Name);
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    if (!observers[i].MakeOffer())
                    {
                        GenaAgent.Unsubscribe(observers[i]);
                        observers.RemoveAt(i);
                    }
                }
                else
                {
                    GenaAgent.Unsubscribe(observers[i]);
                    observers.RemoveAt(i);
                }
                i++;
                if (i > observers.Count - 1) i = 0;
            }
            auction.Congratulations();
        }
    }
}

