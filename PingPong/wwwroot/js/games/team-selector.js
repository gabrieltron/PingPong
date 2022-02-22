$(document).ready(function () {
    $("input[name$='TeamsSize']").click(function () {
        var args = JSON.parse($("[data-role='js-args']").text());
        var teamsSize = $(this).val();
        if (teamsSize== 1) {
            updateTeamSelectors(args.SingleTeams);
        } else {
            updateTeamSelectors(args.DoubleTeams);
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
