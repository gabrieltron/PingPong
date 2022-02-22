$(document).ready(function () {
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
