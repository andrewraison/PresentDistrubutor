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

            //Add all the people who aren't part of the first group
            _ValidPersons.AddRange(_People.SkipLast(_People.Count() - (int)_Groups[0][0]));
            _ValidPersons.AddRange(_People.Skip((int)_Groups[0][1]));


            for (short groupIteration = 0; groupIteration < _Groups.Count() - 2; groupIteration++)
            {
                
            }

            //TODO: final group secial rules

        }

        private void SortGroups()
        {
            //TODO: implement 3 way quick sort
        }
    }

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