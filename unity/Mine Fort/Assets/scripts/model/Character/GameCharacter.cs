
using System;
using System.Collections.Generic;
using System.Xml;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using MineFort.Localization;
using UnityEngine;
using MineFort.Entities.States;
using MineFort.Entities;
using MineFort.model.Inventory;
using MineFort.model;

namespace MineFort.Entities
{
    [MoonSharpUserData]
    public class GameCharacter : MovableEntity, ISelectable, IContextActionProvider, IUpdatable
    {




       

        /// Tiles per second.
        private float speed;
        private float baseSpeed = 5f;

        /// Used for health system.
        private HealthSystem health;

        /// Tile where job should be carried out, if different from MyJob.tile.
        private Tile jobTile;


        private Color characterColor;
        private Color characterUniformColor;
        private Color characterSkinColor;

        // The current state
        private State state;

        // List of global states that always run
        private List<State> globalStates;

        // Queue of states that aren't important enough to interrupt, but should run soon
        private Queue<State> stateQueue;

        /// Use only for serialization
        public GameCharacter():base()
        {

            Needs = new Need[PrototypeManager.Need.Count];
            InitializeCharacterValues();
        }

        public GameCharacter(Tile tile, Color color, Color uniformColor, Color skinColor, string name):base()
        {
            if (tile.Type != TileType.Empty)
                while (tile.Up().Type != TileType.Empty)
                {
                    tile = tile.Up();
                }
            Tile = tile;
            characterColor = color;
            characterUniformColor = uniformColor;
            characterSkinColor = skinColor;
            InitializeCharacterValues();
            Name = name;
            stateQueue = new Queue<State>();
            globalStates = new List<State>
            {
                new NeedState(this)
            };
        }

        /// A callback to trigger when character information changes (notably, the position).
        public event Action<GameCharacter> OnCharacterChanged;

        /// Name of the Character.

        /// The item we are carrying (not gear/equipment).
        public GameInventory Inventory { get; set; }

        /// Holds all character animations.
        public Animation.CharacterAnimation Animation { get; set; }

        /// Is the character walking or idle.
        public bool IsWalking { get; set; }

        /// What direction our character is looking.
        public GameConsts.Facing CharFacing { get; protected set; }


        /// All the needs of this character.
        public Need[] Needs { get; private set; }


      
      

        public Bounds Bounds
        {
            get
            {
                return new Bounds(
                    new Vector3(X - 1, Y - 1, 0),
                    new Vector3(1, 1));
            }
        }

      

        /// Our job, if any.
        public Job MyJob
        {
            get
            {
                JobState jobState = FindInitiatingState() as JobState;
                if (jobState != null)
                {
                    return jobState.Job;
                }

                return null;
            }
        }


        /// <summary>
        /// Gets the Health of this object.
        /// </summary>
        public HealthSystem Health
        {
            get
            {
                if (health == null)
                {
                    health = new HealthSystem(-1f, true, false, false, false);
                }

                return health;
            }
        }

        public IEnumerable<ContextMenuAction> GetContextMenuActions(ContextMenu contextMenu)
        {
            yield return new ContextMenuAction
            {
                LocalizationKey = "Poke " + GetName(),
                RequireCharacterSelected = false,
                Action = (cm, c) => { UnityDebugger.Debugger.Log("Character", GetDescription()); health.CurrentHealth -= 5; }
            };

            yield return new ContextMenuAction
            {
                LocalizationKey = "Heal +5",
                RequireCharacterSelected = false,
                Action = (cm, c) => { health.CurrentHealth += 5; }
            };
        }

        #region State

        public void PrioritizeJob(Job job)
        {
            if (state != null)
            {
                state.Interrupt();
            }

            SetState(new JobState(this, job));
        }

        /// <summary>
        /// Stops the current state. Makes the character halt what is going on and start looking for something new to do, might be the same thing.
        /// </summary>
        public void InterruptState()
        {
            if (state != null)
            {
                state.Interrupt();

                // We can't use SetState(null), because it runs Exit on the state and we don't want to run both Interrupt and Exit.
                state = null;
            }
        }

        /// <summary>
        /// Removes all the queued up states.
        /// </summary>
        public void ClearStateQueue()
        {
            // If we interrupt, we get rid of the queue as well.
            while (stateQueue.Count > 0)
            {
                State queuedState = stateQueue.Dequeue();
                queuedState.Interrupt();
            }
        }

        public void QueueState(State newState)
        {
            stateQueue.Enqueue(newState);
        }

        public void SetState(State newState)
        {
            if (state != null)
            {
                state.Exit();
            }

            state = newState;

            if (state != null)
            {
                state.Enter();
            }
        }

        #endregion

        /// Runs every "frame" while the simulation is not paused
        public void EveryFrameUpdate(float deltaTime)
        {
            // Run all the global states first so that they can interrupt or queue up new states
            foreach (State globalState in globalStates)
            {
                globalState.Update(deltaTime);
            }

            // We finished the last state
            if (state == null)
            {
                if (stateQueue.Count > 0)
                {
                    SetState(stateQueue.Dequeue());
                }
                else
                {
                    Job job = World.Current.jobQueue.GetJob(this);
                    if (job != null)
                    {
                        SetState(new JobState(this, job));
                    }
                    else
                    {
                        // TODO: Lack of job states should be more interesting. Maybe go to the pub and have a pint?
                        SetState(new IdleState(this));
                    }
                }
            }

            state.Update(deltaTime);

            Animation.Update(deltaTime);

            if (OnCharacterChanged != null)
            {
                OnCharacterChanged(this);
            }
        }

        //TODO: mover isso para physicalentity
        public object ToJSON()
        {
            JObject characterJson = new JObject();

            characterJson.Add("Name", Name);
            characterJson.Add("X", Tile.X);
            characterJson.Add("Y", Tile.Y);
            characterJson.Add("Z", Tile.Z);

            JObject needsJSon = new JObject();
            foreach (Need need in Needs)
            {
                needsJSon.Add(need.Type, need.Amount);
            }

            characterJson.Add("Needs", needsJSon);

            JObject colorsJson = new JObject();
            colorsJson.Add("CharacterColor", new JArray(characterColor.r, characterColor.g, characterColor.b));
            colorsJson.Add("UniformColor", new JArray(characterUniformColor.r, characterUniformColor.g, characterUniformColor.b));
            colorsJson.Add("SkinColor", new JArray(characterSkinColor.r, characterSkinColor.g, characterSkinColor.b));
            characterJson.Add("Colors", colorsJson);

            JObject statsJSon = new JObject();
            foreach (string stat in Stats.Keys)
            {
                statsJSon.Add(stat, Stats[stat].Value);
            }

            characterJson.Add("Stats", statsJSon);

            if (Inventory != null)
            {
                characterJson.Add("Inventories", new JArray(Inventory.ToJSon()));
            }

            return characterJson;
        }

        #region ISelectableInterface implementation

        

        public override string GetDescription()
        {
            return "A human.";
        }

        public override IEnumerable<string> GetAdditionalInfo()
        {
            yield return health.TextForSelectionPanel();

            foreach (Need n in Needs)
            {
                yield return LocalizationTable.GetLocalization(n.LocalizationID, n.DisplayAmount);
            }

            foreach (string stat in Stats.Keys)
            {
                yield return LocalizationTable.GetLocalization("stat_" + stat.ToLower(), Stats[stat].Value);
            }
        }

        public Color GetCharacterColor()
        {
            return characterColor;
        }

        public Color GetCharacterSkinColor()
        {
            return characterSkinColor;
        }

        public Color GetCharacterUniformColor()
        {
            return characterUniformColor;
        }

        public override string GetJobDescription()
        {
            if (MyJob == null)
            {
                return "job_no_job_desc";
            }

            return MyJob.Description;
        }

        #endregion

        public void FaceTile(Tile nextTile)
        {
            // Find character facing
            if (nextTile.X > Tile.X)
            {
                CharFacing = GameConsts.Facing.EAST;
            }
            else if (nextTile.X < Tile.X)
            {
                CharFacing = GameConsts.Facing.WEST;
            }
            else if (nextTile.Y > Tile.Y)
            {
                CharFacing = GameConsts.Facing.NORTH;
            }
            else
            {
                CharFacing = GameConsts.Facing.SOUTH;
            }
        }

        public void FixedFrequencyUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }

        private State FindInitiatingState()
        {
            if (state == null)
            {
                return null;
            }

            State rootState = state;
            while (rootState.NextState != null)
            {
                rootState = rootState.NextState;
            }

            return rootState;
        }

        private void InitializeCharacterValues()
        {
            LoadNeeds();
            LoadStats();
            UseStats();
        }

        private void LoadNeeds()
        {
            Needs = new Need[PrototypeManager.Need.Count];
            PrototypeManager.Need.Values.CopyTo(Needs, 0);
            for (int i = 0; i < PrototypeManager.Need.Count; i++)
            {
                Need need = Needs[i];
                Needs[i] = need.Clone();
                Needs[i].Character = this;
            }
        }

        private void LoadStats()
        {
            Stats = new Dictionary<string, Stat>(PrototypeManager.Stat.Count);
            for (int i = 0; i < PrototypeManager.Stat.Count; i++)
            {
                Stat prototypeStat = PrototypeManager.Stat.Values[i];
                Stat newStat = prototypeStat.Clone();

                // Gets a random value within the min and max range of the stat.
                // TODO: Should there be any bias or any other algorithm applied here to make stats more interesting?
                newStat.Value = UnityEngine.Random.Range(1, 20);
                Stats.Add(newStat.Type, newStat);
            }

            UnityDebugger.Debugger.Log("Character", "Initialized " + Stats.Count + " Stats.");
        }

        /// <summary>
        /// Use the stats of the character to determine various traits.
        /// </summary>
        private void UseStats()
        {
            // The speed is equal to (baseSpeed +/-30% of baseSpeed depending on Dexterity)
            speed = baseSpeed + (0.3f * baseSpeed * ((float)Stats["Dexterity"].Value - 10) / 10);

            // Base character max health on their constitution.
            health = new HealthSystem(50 + ((float)Stats["Constitution"].Value * 5));
        }
    }
}

