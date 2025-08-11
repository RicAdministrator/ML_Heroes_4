app.controller("heroCtrl", function ($scope, $http) {
    $scope.addClicked = function () {
        $scope.resetSearchMessages();
        $scope.activeSection = 'upsert';
    }

    $scope.resetUpsertForm = function () {
        $scope.heroId = null;

        $scope.nameModel = "";
        $scope.imageUrlModel = "";
        $scope.descriptionModel = "";
        $scope.roleIdsModel = {};

        $scope.saveErrors = [];
    }

    $scope.getHeroesData = function () {
        $http.get("https://localhost:7179/api/Hero")
            .then(function (response) {
                $scope.heroesData = response.data;
            }, function (error) {
                console.error("Failed to fetch heroes:", error);
                $scope.heroesData = [];
            });
    }

    $scope.getRolesData = function () {
        $http.get("https://localhost:7179/api/Role")
            .then(function (response) {
                $scope.rolesData = response.data;
            }, function (error) {
                console.error("Failed to fetch roles:", error);
                $scope.rolesData = [];
            });
    }

    $scope.getRoleNames = function (hero) {
        if (!hero.roles || !hero.roles.length) return '';
        return hero.roles.map(function (r) { return r.heroRole; }).join(' / ');
    };

    $scope.resetSearchMessages = function () {
        $scope.successMsg = "";
    }

    $scope.updateClicked = function (id, name, imageUrl, description, roles) {
        $scope.resetSearchMessages();

        $scope.heroId = id;
        $scope.nameModel = name;
        $scope.imageUrlModel = imageUrl;
        $scope.descriptionModel = description;
        $scope.roleIdsModel = {};
        if (Array.isArray(roles)) {
            roles.forEach(function (role) {
                $scope.roleIdsModel[role.id] = role.id;
            });
        }

        $scope.activeSection = 'upsert';
    }

    $scope.cancelClicked = function () {
        $scope.resetUpsertForm();
        $scope.activeSection = 'search';
    }

    $scope.saveClicked = function () {
        $scope.saveErrors = [];

        if ($scope.nameModel === "") {
            $scope.saveErrors.push("Name is required.");
        }
        else {
            const duplicateHero = $scope.heroesData.filter(hero => hero.name.toLowerCase() === $scope.nameModel.trim().toLowerCase() && hero.id !== $scope.heroId);

            if (duplicateHero.length > 0) {
                $scope.saveErrors.push("Hero with this name already exists.");
            }
        }

        const selectedRoleIds = [];
        angular.forEach($scope.roleIdsModel, function (value, key) {
            if (value) selectedRoleIds.push(parseInt(key));
        });

        if (selectedRoleIds.length === 0) {
            $scope.saveErrors.push("At least one role must be selected.");
        }

        if ($scope.saveErrors.length > 0) {
            return; // If there are errors, do not proceed with saving
        }

        const heroData = {
            name: $scope.nameModel,
            imageUrl: $scope.imageUrlModel,
            description: $scope.descriptionModel,
            roleIds: selectedRoleIds
        };

        if ($scope.heroId) {
            // Update existing hero
            $http.put("https://localhost:7179/api/Hero/" + $scope.heroId, heroData)
                .then(function (response) {
                    $scope.successMsg = "Hero updated successfully!";
                    $scope.resetUpsertForm();
                    $scope.activeSection = 'search';
                    // Refresh heroes data
                    return $http.get("https://localhost:7179/api/Hero");
                })
                .then(function (response) {
                    $scope.heroesData = response.data;
                })
                .catch(function (error) {
                    console.error("Failed to update hero:", error);
                    alert("Error updating hero.");
                });
        } else {
            // Add new hero
            $http.post("https://localhost:7179/api/Hero", heroData)
                .then(function (response) {
                    $scope.successMsg = "Hero added successfully!";
                    $scope.resetUpsertForm();
                    $scope.activeSection = 'search';
                    // Refresh heroes data
                    return $http.get("https://localhost:7179/api/Hero");
                })
                .then(function (response) {
                    $scope.heroesData = response.data;
                })
                .catch(function (error) {
                    console.error("Failed to add hero:", error);
                    alert("Error adding hero.");
                });
        }
    }

    $scope.deleteClicked = function (id) {
        $scope.resetSearchMessages();

        if (confirm("Are you sure you want to delete this hero?")) {
            $http.delete("https://localhost:7179/api/Hero/" + id)
                .then(function (response) {
                    $scope.successMsg = "Hero deleted successfully!";
                    // Refresh heroes data
                    return $http.get("https://localhost:7179/api/Hero");
                })
                .then(function (response) {
                    $scope.heroesData = response.data;
                    window.scrollTo({ top: 0, behavior: "smooth" });
                })
                .catch(function (error) {
                    console.error("Failed to delete hero:", error);
                    alert("Error deleting hero.");
                });
        }
    }

    $scope.highlightNavigation = function () {
        const navHeroes = document.getElementById("navHeroes");
        if (navHeroes) navHeroes.classList.add("w3-blue");

        const navHeroesSmall = document.getElementById("navHeroesSmall");
        if (navHeroesSmall) navHeroesSmall.classList.add("w3-blue");

        const navRoles = document.getElementById("navRoles");
        if (navRoles) navRoles.classList.remove("w3-blue");

        const navRolesSmall = document.getElementById("navRolesSmall");
        if (navRolesSmall) navRolesSmall.classList.remove("w3-blue");
    }

    $scope.heroesData = [];
    $scope.getHeroesData();
    $scope.rolesData = [];
    $scope.getRolesData();

    $scope.highlightNavigation();

    $scope.activeSection = "search";

    $scope.nameModel = "";
    $scope.imageUrlModel = "";
    $scope.roleIdsModel = {};
    $scope.descriptionModel = "";

    $scope.heroId = null;

    $scope.saveErrors = [];
    $scope.successMsg = "";
});