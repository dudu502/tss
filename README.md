# FiniteStateMachine
Implement finite state machine


``` csharp
            // Create a state machine
            StateMachine<State, StateObject>.Log = Console.WriteLine;
            var machine = new StateMachine<State,StateObject>(new StateObject());
            machine
                .NewState(State.Idle)
                    .Initialize((stateObj) => { })
                    .Enter((stateObj) => { })            
                    .Update((stateObj) =>
                    {
                        stateObj.physical_strength++;
                        stateObj.Log();
                    })
                    .Translate((stateObj) => stateObj.physical_strength >= StateObject.MAX_PHYSICAL_STRENGTH).To(State.Run)
                    .Exit((stateObj) => { })             
                .End()
                .NewState(State.Run)
                    .Initialize((stateObj) => { })
                    .Enter((stateObj) => { })
                    .Update((stateObj) =>
                    {
                        stateObj.physical_strength--;
                        stateObj.Log();
                    })
                    .Translate((stateObj) => stateObj.physical_strength <= 0).To(State.Idle)
                    .Exit((stateObj) => { })
                .End()
                .Initialize().Start(State.Idle);

            bool running = true;
            ThreadPool.QueueUserWorkItem(_ => { var key = Console.ReadKey(); running = false; });
            while (running)
            {
                machine.Update();
                Thread.Sleep(100);
            }
            
            machine.Stop();
            Console.WriteLine("FSM Stop");
```


``` logs


Idle OnInitialize
Run OnInitialize
Idle m_OnEnter
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 26
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 27
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 28
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 29
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 30
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 31
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 32
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 33
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 34
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 35
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 36
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 37
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 38
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 39
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 40
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 41
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 42
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 43
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 44
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 45
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 46
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 47
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 48
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 49
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 50
State:Idle ToState:Run OnValid True
Idle OnExit
Run m_OnEnter
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 49
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 48
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 47
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 46
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 45
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 44
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 43
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 42
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 41
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 40
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 39
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 38
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 37
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 36
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 35
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 34
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 33
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 32
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 31
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 30
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 29
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 28
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 27
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 26
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 25
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 24
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 23
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 22
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 21
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 20
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 19
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 18
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 17
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 16
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 15
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 14
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 13
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 12
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 11
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 10
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 9
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 8
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 7
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 6
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 5
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 4
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 3
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 2
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 1
State:Run ToState:Idle OnValid False
Run OnUpdate
Current physical_strength 0
State:Run ToState:Idle OnValid True
Run OnExit
Idle m_OnEnter
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 1
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 2
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 3
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 4
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 5
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 6
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 7
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 8
State:Idle ToState:Run OnValid False
Idle OnUpdate
Current physical_strength 9
FSM Stop
```