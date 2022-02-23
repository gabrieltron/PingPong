$(document).ready(function () {
    var args = JSON.parse($("[data-role='js-args']").text());
    var singleTeams = args.SingleTeams;
    var doubleTeams = args.DoubleTeams;

    $("input[name$='TeamsSize']").click(function () {
        var teamsSize = $(this).val();
        if (teamsSize== 1) {
            updateTeamSelectors(singleTeams);
        } else {
            updateTeamSelectors(doubleTeams);
        }
    });

    function updateTeamSelectors(teams) {
        $teamSelectors = $(".team-selector");
        $teamSelectors.empty();
        $teamSelectors.append($("<option></option>").attr("value", "").text("Select Team"));
        teams.forEach(function (team) {
            $teamSelectors.append($("<option></option>").attr("value", team.Id).text(team.Name));
        });
        $teamSelectors.val("");

    }
});
