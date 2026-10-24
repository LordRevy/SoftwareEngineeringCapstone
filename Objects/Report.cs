using System.ComponentModel.DataAnnotations;

namespace AUBE.Objects;

/// <summary>
/// Represents a Report Object. Holds simulation data for recordkeeping and analysis.
/// </summary>
public class Report
{
    public required int Id              { get; set; }                       // Unique identifier for the Report
    public required int T1Id            { get; set; }                       // ID of Team 1.
    public required string T1Name       { get; set; }                       // Name of Team 1.
    public required int T2Id            { get; set; }                       // ID of Team 2.
    public required string T2Name       { get; set; }                       // Name of Team 2.
    public required DateTime CreatedAt  { get; set; }                       // The UTC that the Report was created at.
    public int Iterations               { get; set; }                       // Amount of times the simulation ran.
    public int T1Wins                   { get; set; }                       // Amount of times Team 1 won.
    public int T1TotalRoundsToWin       { get; set; }                       // Total amount of rounds taken for Team 1 to win.
    public int T2Wins                   { get; set; }                       // Amount of times Team 2 won.
    public int T2TotalRoundsToWin       { get; set; }                       // Total amount of rounds taken for Team 2 to win.
    public int Draws                    { get; set; }                       // Edge case if both Units lose on same round somehow.
    public int T1TotalHealthLostOnWin   { get; set; }                       // Total amount of health lost for Team 1 on a win.
    public int T2TotalHealthLostOnWin   { get; set; }                       // Total amount of health lose for Team 2 on a win.
    public int T1TotalAttacks           { get; set; }                       // Total attacks Team 1 made.
    public int T1TotalDodgedAttacks     { get; set; }                       // Total number of attacks dodged by Team 1.
    public int T2TotalAttacks           { get; set; }                       // Total attacks Team 2 made.
    public int T2TotalDodgedAttacks     { get; set; }                       // Total number of attacks dodged by Team 2.
    public int T1TotalHits              { get; set; }                       // Total number of hits by Team 1.
    public int T1TotalBlockedHits       { get; set; }                       // Total number of hits blocked by Team 1.
    public int T2TotalHits              { get; set; }                       // Total number of hits by Team 2.
    public int T2TotalBlockedHits       { get; set; }                       // Total number of hits blocked by Team 2.
    public int T1TotalCriticalHits      { get; set; }                       // Total number of critical hits by Team 1.
    public int T2TotalCriticalHits      { get; set; }                       // Total number of Critical hits by Team 2.
    public int T1TotalDamageDealt       { get; set; }                       // Total Damage dealt by Team 1.
    public int T2TotalDamageDealt       { get; set; }                       // Total Damage dealt by Team 2.
    public int T1TotalArmorApplied      { get; set; }                       // Total amount of damage avoided with armor by Team 1.
    public int T2TotalArmorApplied      { get; set; }                       // Total amount of damage avoided with armor by Team 2.
}