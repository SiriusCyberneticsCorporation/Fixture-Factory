using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory
{
	public class FixtureCalculator
	{
		private const int BYE = -1;

		public Dictionary<int, List<KeyValuePair<int, int>>> GenerateFixtures(int num_teams)
		{
			int rounds = num_teams;
			int[] teams = new int[num_teams];
			int[,] fixtures = GenerateRoundRobin(num_teams);
			Dictionary<int, List<KeyValuePair<int, int>>> result = new Dictionary<int, List<KeyValuePair<int, int>>>();

			if (num_teams % 2 == 0)
			{
				rounds--;
			}

			for (int index = 0; index < num_teams; index++)
			{
				teams[index] = index;
			}

			for (int round = 0; round < rounds; round++) 
			{
				int byeIndex = -1;
				for (int index = 0; index < num_teams; index++)
				{
					int home = teams[index];
					int away = fixtures[teams[index], round];

					if(away == BYE)
					{
						byeIndex = teams[index];
					}
					else
					{
						if(!result.ContainsKey(round))
						{
							result.Add(round, new List<KeyValuePair<int, int>>());
						}

						bool add = true;
						foreach (KeyValuePair<int, int> teamPair in result[round])
						{
							if(teamPair.Key == home || teamPair.Key == away || teamPair.Value == home || teamPair.Value == away)
							{
								add = false;
								break;
							}
						}
						if(add)
						{
							result[round].Add(new KeyValuePair<int, int>(home, away));
						}
					}
				}
				RotateArray(teams);
				if (byeIndex >=0)
				{
					result[round].Add(new KeyValuePair<int, int>(byeIndex, -1));
				}
			}

			return result;
		}

		private int[,] GenerateRoundRobin(int num_teams)
		{
			if (num_teams % 2 == 0)
			{
				return GenerateRoundRobinEven(num_teams);
			}
			else
			{
				return GenerateRoundRobinOdd(num_teams);
			}
		}

		// Return an array where results(i, j) gives
		// the opponent of team i in round j.
		// Note: num_teams must be odd.
		private int[,] GenerateRoundRobinOdd(int num_teams)
		{
			int n2 = (int)((num_teams - 1) / 2);
			int[,] results = new int[num_teams, num_teams];

			// Initialize the list of teams.
			int[] teams = new int[num_teams];
			for (int i = 0; i < num_teams; i++) teams[i] = i;

			// Start the rounds.
			for (int round = 0; round < num_teams; round++)
			{
				for (int i = 0; i < n2; i++)
				{
					int team1 = teams[n2 - i];
					int team2 = teams[n2 + i + 1];
					results[team1, round] = team2;
					results[team2, round] = team1;
				}

				// Set the team with the bye.
				results[teams[0], round] = BYE;

				// Rotate the array.
				RotateArray(teams);
			}

			return results;
		}

		private int[,] GenerateRoundRobinEven(int num_teams)
		{
			// Generate the result for one fewer teams.
			int[,] results = GenerateRoundRobinOdd(num_teams - 1);

			// Copy the results into a bigger array,
			// replacing the byes with the extra team.
			int[,] results2 = new int[num_teams, num_teams - 1];
			for (int team = 0; team < num_teams - 1; team++)
			{
				for (int round = 0; round < num_teams - 1; round++)
				{
					if (results[team, round] == BYE)
					{
						// Change the bye to the new team.
						results2[team, round] = num_teams - 1;
						results2[num_teams - 1, round] = team;
					}
					else
					{
						results2[team, round] = results[team, round];
					}
				}
			}

			return results2;
		}

		// Rotate the entries one position.
		private void RotateArray(int[] teams)
		{
			int tmp = teams[teams.Length - 1];
			Array.Copy(teams, 0, teams, 1, teams.Length - 1);
			teams[0] = tmp;
		}
	}
}
