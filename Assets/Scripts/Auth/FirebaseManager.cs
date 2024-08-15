using UnityEngine;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using static FirebaseManager;
using System;
using System.Collections;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections.Generic;

public class FirebaseManager : Singleton<FirebaseManager>
{
    // Reference to the Firebase database
    private DatabaseReference mDatabase;

    private void Awake()
    {
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

    }

    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                // Get a reference to the database
                mDatabase = FirebaseDatabase.DefaultInstance.RootReference;

                if (!IsUserLoggedIn()) // if user isn't logged in, login anonymously
                                       // Sign in anonymously
                {
                    SignInAnonymously();                    
                }
                else
                {
                    SetCurrentUserFromId(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
                }
            }
        });
    }

    /// <summary>
    /// get user data from the logged in user or set the user as default user
    /// </summary>
    /// <param name="userId"></param>
    public void SetCurrentUserFromId(string userId)
    {
        mDatabase.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to read data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    SetLoggedInUser(snapshot);
                }
                else
                {
                    // first time user, initialize a user entry in the user db
                    mDatabase.Child("users").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(GameManager.Instance.defaultUser))
                            .ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    // Handle the error
                                    Debug.LogError("Error saving default user: " + task.Exception.Message);
                                    // You might want to display an error message to the user
                                }
                                else if (task.IsCompletedSuccessfully)
                                {
                                    // Handle success
                                    Debug.Log("Default user saved successfully!");
                                }
                            });
                }
            }
        });
    }

    public void UpdatePlayMenu()
    {
        List<Election> elections = new List<Election>();

        // Get a reference to the "users" node
        var electionsRef = mDatabase.Child("elections");

        // Attach a listener to the "users" node
        electionsRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching elections: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                // Get the snapshot of the data
                DataSnapshot snapshot = task.Result;

                // Iterate through each user node
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    // Create a User object from the data
                    Election election = ConvertSnapshotToElection(childSnapshot);
                    elections.Add(election);

                }
            }

            UnityMainThreadDispatcher.Instance().Enqueue(GameManager.Instance.PopulatePlayMenu(elections));
        });
    }

    private void UpdateWinnerVoteCount(Election election, Party winningParty)
    {
        if (winningParty == Party.Democrat)
            election.demVotes++;
        if (winningParty == Party.Republican)
            election.repVotes++;
    }

    /// <summary>
    /// get user data from the logged in user
    /// </summary>
    /// <param name="userId"></param>
    public void SetElectionVictor(Party winningParty, int electionYear, Swag swag)
    {
        if (winningParty == Party.None && swag == Swag.None)
            return;

        mDatabase.Child("elections").Child(electionYear.ToString()).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to read data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                Election election;
                if (snapshot.Exists)
                {
                    election = ConvertSnapshotToElection(snapshot);
                }
                else
                {
                    // tODO: REMOVE MANUALLY ENTER ALL ELECTION YEARS ENTRIES PREVENT ANY GLOBAL MODIFICATION RESET
                    election = new Election(electionYear, 0, 0, (int)Swag.None, (int)Swag.None);
                }

                UpdateWinnerVoteCount(election, winningParty);
                UpdateSwag(election, winningParty, swag);

                // initialize the Election in the database
                UpdateElection(election);
            }
        });
    }

    void UpdateSwag(Election existingElection, Party winningParty, Swag swag)
    {
        if (swag != Swag.None && winningParty == Party.Democrat)
            existingElection.demSwag = (int)swag;
        if (swag != Swag.None && winningParty == Party.Republican)
            existingElection.repSwag = (int)swag;
    }

    // updates should synchronize with player pref changes
    public void UpdateElection(Election election)
    {
        try
        {
            FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

            if (currentUser != null) // if become popular implement user.isemailverified
            {
                mDatabase.Child("elections").Child(election.electionYear.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(election))
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            // Handle the error
                            Debug.LogError("Error updating election: " + task.Exception.Message);
                            // You might want to display an error message to the user
                        }
                        else if (task.IsCompletedSuccessfully)
                        {
                            // Handle success
                            Debug.Log("Election updated successfully!");
                        }
                    });
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to update user, may be offline");
        }
    }

    //public void SetElectionResult(DataSnapshot snapshot)
    //{
    //    Election election = ConvertSnapshotToElection(snapshot);


    //}

    public void SetLoggedInUser(DataSnapshot snapshot)
    {
        User loggedinUser = ConvertSnapshotToUser(snapshot);

        UnityMainThreadDispatcher.Instance().Enqueue(GameManager.Instance.SetUser(loggedinUser));
    }

    public bool IsUserLoggedIn()
    {
        // Get the current user
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            // User is logged in

            Debug.Log("User is logged in: " + user.DisplayName);

            // Do something for logged-in users (e.g., show a welcome message)
            return true;
        }
        else
        {
            // User is not logged in
            Debug.Log("User is not logged in.");
            // Do something for non-logged-in users (e.g., show a login screen)
            return false;
        }
    }

    // Sign in anonymously
    private void SignInAnonymously()
    {      
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                // Get the user's UID
                string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                SetCurrentUserFromId(userId);
                Debug.Log("Anonymous user signed in with UID: " + userId);
            }
        });
    }

    private Election ConvertSnapshotToElection(DataSnapshot snapshot)
    {
        int electionY = int.Parse(snapshot.Child("electionYear").GetValue(true)?.ToString());
        int demVotes = int.Parse(snapshot.Child("demVotes")?.GetValue(true)?.ToString());
        int repVotes = int.Parse(snapshot.Child("repVotes")?.GetValue(true)?.ToString());
        int demSwag = int.Parse(snapshot.Child("demSwag")?.GetValue(true)?.ToString());
        int repSwag = int.Parse(snapshot.Child("repSwag")?.GetValue(true)?.ToString());
        return new Election(electionY, demVotes, repVotes, demSwag, repSwag);
    }

    private User ConvertSnapshotToUser(DataSnapshot snapshot)
    {
        string username = snapshot.Child("username").GetValue(true).ToString();
        //string profilePic = snapshot.Child("profilePic")?.GetValue(true)?.ToString();
        //int score = int.Parse(snapshot.Child("score")?.GetValue(true)?.ToString());
        //int highScore = int.Parse(snapshot.Child("highScore")?.GetValue(true)?.ToString());
        //bool boughtCheckpoint = GetBoolOrDefault(snapshot.Child("boughtCheckpoint")?.GetValue(true));
        bool boughtAds = GetBoolOrDefault(snapshot.Child("boughtAds")?.GetValue(true));
        //bool boughtBundle = GetBoolOrDefault(snapshot.Child("boughtBundle")?.GetValue(true));
        //bool boughtCustom = GetBoolOrDefault(snapshot.Child("boughtCustom")?.GetValue(true));

        return new User(username, boughtAds);
    }

    private bool GetBoolOrDefault(object o)
    {
        if (o is bool)
        {
            return (bool)o;
        }
        else
        {
            return false;
        }
    }

    // updates should synchronize with player pref changes
    public void UpdateUser(User user)
    {
        try
        {
            FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

            if (currentUser != null) // if become popular implement user.isemailverified
            {
                mDatabase.Child("users").Child(currentUser.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(user))
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            // Handle the error
                            Debug.LogError("Error updating user: " + task.Exception.Message);
                            // You might want to display an error message to the user
                        }
                        else if (task.IsCompletedSuccessfully)
                        {
                            // Handle success
                            Debug.Log("User updated successfully!");
                        }
                    });
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to update user, may be offline");
        }
    }

    public User CreateCopyOfUser(User user)
    {
        return new User(user.username, user.hasAds);
    }

    // User class to store user data
    [System.Serializable]
    public class User
    {
        public string username;
        public bool hasAds;

        public User(string username, bool hasAds)
        {
            this.username = username;
            this.hasAds = hasAds;
        }
    }

    [System.Serializable]
    public class Election
    {
        public int electionYear;
        public int demVotes;
        public int repVotes;
        public int demSwag;
        public int repSwag;

        public Election(int electionYear, int demVotes, int repVotes, int demSwag, int repSwag)
        {
            this.electionYear = electionYear;
            this.demVotes = demVotes;
            this.repVotes = repVotes;
            this.demSwag = demSwag;
            this.repSwag = repSwag;
        }
    }
}
