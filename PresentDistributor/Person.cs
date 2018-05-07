using System;
using System.Collections.Generic;
using System.Linq;


namespace PresentDistributor
{
    public class Person
    {
        private string _Name;
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }

        private Person _Assignee;
        public Person Assignee
        {
            set { _Assignee = value; }
            get { return _Assignee; }
        }

        /*
        private List<Person> _Partners;
        public void AddPartners(List<Person> partnersToAdd)
        {
            _Partners.AddRange(partnersToAdd);
        }
        public void AddPartners(Person partnersToAdd)
        {
            _Partners.Add(partnersToAdd);
        }
        public List<Person> GetPartners()
        {
            return _Partners;
        }
        public void ClearPartners()
        {
            _Partners = new List<Person>();
        }*/


        public Person(string name)
        {
            _Name = name;
        }


    }

    public class RENAMETHIS
    {
        private Person[] _People;
        public Person[] GetPeople()
        {
            return _People;
        }

        private short[][] _Groups;
        public short[][] GetGroups()
        {
            return _Groups;
        }

        private List<Person> _ValidPersons;

        public void AddGroup(List<Person> newPeople)
        {
            List<Person> newList = _People.ToList();

            int newCount = newPeople.Count;
            int oldCount = newList.Count();

            newList.AddRange(newPeople);
            _People = newList.ToArray();

            short[] newGroup=Enumerable.Range(oldCount, newCount).Select(x => Convert.ToInt16(x)).ToArray();
            List<short[]> revisedGroups = _Groups.ToList();
            revisedGroups.Add(newGroup);
            _Groups = revisedGroups.ToArray();
        }

        public RENAMETHIS (List<List<string>> names)
        {
            List<Person> People = new List<Person>();
            List<short[]> Groups= new List<short[]>();
            foreach(List<string> group in names)
            {
                int oldCount = People.Count;
                foreach(string name in group)
                {
                    People.Add(new Person(name));
                }
                int newCount = People.Count;

                Groups.Add(Enumerable.Range(oldCount, newCount).Select(x => Convert.ToInt16(x)).ToArray());
            }

            _People = People.ToArray();
            _Groups = Groups.ToArray();
        }


        public void SetAssignments()
        {
            //Sort groups asc
            SortGroups();

            if (_People.Count()/2 < _Groups.Last().Count())
            {
                throw new NoSolutionPossibleException(
                    $"The largest group is too large. It must be less than half the total people. Largest group: {_Groups.Last().Count()}, Total people: {_People.Count()}");
            }
            if (_People.Count() < 2)
            {
                throw new NoSolutionPossibleException(
                    $"There aren't enough people. Total people: {_People.Count()}");
            }

            _ValidPersons = new List<Person>();

            //Add all the people who aren't part of the first group
            _ValidPersons.AddRange(_People.SkipLast(_People.Count() - (int)_Groups[0][0]));
            _ValidPersons.AddRange(_People.Skip((int)_Groups[0][1]));

            List<Person> removedPersons = new List<Person>();

            removedPersons.AddRange(_People.Skip((int)_Groups[0][0]).Take((int)_Groups[0][1] - (int)_Groups[0][0]));

            Random rnd = new Random();

            //iterate over each group except the last one
            for (short groupIteration = 0; groupIteration < _Groups.Count() - 2; groupIteration++)
            {
                for (short personIteration = _Groups[groupIteration][0]; personIteration < _Groups[groupIteration][1]; personIteration++)
                {
                    int assigneeIndex = rnd.Next(_ValidPersons.Count);
                    _People[personIteration].Assignee = _ValidPersons.ElementAt(assigneeIndex);

                    _ValidPersons.RemoveAt(assigneeIndex);
                }

                //TODO: Set up validpersons for next group
                _ValidPersons.InsertRange(PreviousIndexInPeople(_Groups[groupIteration][0]), removedPersons);
                _ValidPersons.Select(x => )
            }

            //TODO: final group secial rules

        }

        private int PreviousIndexInPeople(int personIndex)
        {
            while (personIndex != 0)
            {
                personIndex--;

                if (_ValidPersons.IndexOf(_People[personIndex]) != -1)
                {
                    return _ValidPersons.IndexOf(_People[personIndex]);
                }
            }

            return 0;
        }

        private void SortGroups()
        {
            //TODO: implement 3 way quick sort
        }
    }

    /*public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
            foreach (T item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }
    }*/

    [Serializable()]
    public class NoSolutionPossibleException : Exception
    {
        public NoSolutionPossibleException() : base() { }
        public NoSolutionPossibleException(string message) : base(message) { }
        public NoSolutionPossibleException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected NoSolutionPossibleException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}