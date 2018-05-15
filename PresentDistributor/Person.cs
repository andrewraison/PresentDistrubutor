using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;


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

        private Person _Giver;
        public Person Giver
        {
            set { _Giver = value; }
            get { return _Giver; }
        }

        public Person(string name)
        {
            _Name = name;
        }


    }



    public class Assignments
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

        public Assignments (List<List<string>> names)
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
                    _ValidPersons.ElementAt(assigneeIndex).Giver = _People[personIteration];

                    _ValidPersons.RemoveAt(assigneeIndex);
                }

                //put the removed persons back into _validPersons, and remove the next group of people
                _ValidPersons.InsertRange(PreviousIndexInPeople(_Groups[groupIteration][0]), removedPersons);
                removedPersons = _ValidPersons.Where(x => IsPersonInGroup(groupIteration+1, x)).ToList();
            }

            //TODO: final group secial rules
            bool isReassigning = false;
            for (short personIteration = _Groups[_Groups.Length - 1][0]; personIteration < _Groups[_Groups.Length - 1][1]; personIteration++)
            {
                if (_ValidPersons.Count != 0)
                {
                    isReassigning = true;
                    _ValidPersons = _People.SkipLast(_Groups[_Groups.Length - 1][1] - _Groups[_Groups.Length - 1][0]).ToList();
                }
                if (!isReassigning)
                {
                    int assigneeIndex = rnd.Next(_ValidPersons.Count);
                    _People[personIteration].Assignee = _ValidPersons.ElementAt(assigneeIndex);
                    _ValidPersons.ElementAt(assigneeIndex).Giver = _People[personIteration];

                    _ValidPersons.RemoveAt(assigneeIndex);
                }
                else
                {
                    int assigneeIndex = rnd.Next(_ValidPersons.Count);
                    Person oldGiver = _ValidPersons.ElementAt(assigneeIndex).Giver;
                    _People[personIteration].Assignee = _ValidPersons.ElementAt(assigneeIndex);
                    _ValidPersons.ElementAt(assigneeIndex).Giver = _People[personIteration];
                    _ValidPersons.RemoveAt(assigneeIndex);

                    assigneeIndex = rnd.Next(removedPersons.Count);
                    oldGiver.Assignee = removedPersons.ElementAt(assigneeIndex);
                    removedPersons.ElementAt(assigneeIndex).Giver = oldGiver;
                    removedPersons.RemoveAt(assigneeIndex);
                }
            }

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

        private bool IsPersonInGroup(int groupIndex, Person person)
        {
            for (short personIndex = _Groups[groupIndex][0]; personIndex <= _Groups[groupIndex][1]; personIndex++)
            {
                if (person == _People[personIndex])
                {
                    return true;
                }
            }
            return false;
        }

        private void SortGroups()
        {
            int[] keys = new int[_Groups.Length];

            for (int i = 0; i < _Groups.Length; i++)
            {
                keys[i] = _Groups[i][1] - _Groups[i][0];
            }

            //sort keys
            ThreeWayQuickSort.QuickSort(ref keys, 0, 0);

            short[][] tmpGroup = new short[_Groups.Length][2];

            //use keys to sort groups
            for (int i = 0; i < _Groups.Length; i++)
            {
                for (int j = 0; j < keys.Length; j++)
                {
                    if (keys[j] == _Groups[i][1] - _Groups[i][0])
                    {
                        tmpGroup[j][0] = _Groups[i][0];
                        tmpGroup[j][1] = _Groups[i][1];
                        keys[j] = -1;
                        break;
                    }
                }
            }

            _Groups = tmpGroup;
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