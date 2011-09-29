// Copyright 2011 Chris Patterson, Dru Sellers
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;


    [TestFixture]
    public class Introspection_Specs
    {
        [Test]
        public void The_machine_shoud_report_its_instance_type()
        {
            Assert.AreEqual(typeof(Instance), _machine.InstanceType);
        }

        [Test]
        public void The_machine_should_expose_all_events()
        {
            Assert.AreEqual(4, ((StateMachine)_machine).Events.Count());
            Assert.Contains(_machine.Ignored, _machine.Events.ToList());
            Assert.Contains(_machine.Handshake, _machine.Events.ToList());
            Assert.Contains(_machine.Hello, _machine.Events.ToList());
            Assert.Contains(_machine.YelledAt, _machine.Events.ToList());
        }

        [Test]
        public void The_machine_should_expose_all_states()
        {
            Assert.AreEqual(5, ((StateMachine)_machine).States.Count());
            Assert.Contains(_machine.Initial, _machine.States.ToList());
            Assert.Contains(_machine.Completed, _machine.States.ToList());
            Assert.Contains(_machine.Greeted, _machine.States.ToList());
            Assert.Contains(_machine.Loved, _machine.States.ToList());
            Assert.Contains(_machine.Pissed, _machine.States.ToList());
        }

        [Test]
        public void The_next_events_should_be_known()
        {
            IEnumerable<Event> events = _machine.NextEvents(_instance);
            Assert.AreEqual(3, events.Count());
        }

        Instance _instance;
        TestStateMachine _machine;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _instance = new Instance();
            _machine = new TestStateMachine();

            _machine.RaiseEvent(_instance, _machine.Hello);
        }


        class A
        {
        }


        class B
        {
        }


        class Instance :
            StateMachineInstance
        {
            public State CurrentState { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Hello);
                Event(() => YelledAt);
                Event(() => Handshake);
                Event(() => Ignored);

                State(() => Greeted);
                State(() => Loved);
                State(() => Pissed);

                Initially(
                          When(Hello)
                              .TransitionTo(Greeted));

                During(Greeted,
                    When(Handshake)
                        .TransitionTo(Loved),
                    When(Ignored)
                        .TransitionTo(Pissed));

                DuringAny(When(YelledAt).TransitionTo(Completed));
            }

            public State Greeted { get; set; }
            public State Pissed { get; set; }
            public State Loved { get; set; }

            public Event Hello { get; private set; }
            public Event YelledAt { get; private set; }
            public Event<A> Handshake { get; private set; }
            public Event<B> Ignored { get; private set; }
        }
    }
}