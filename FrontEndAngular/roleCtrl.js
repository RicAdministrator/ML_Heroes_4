app.controller("roleCtrl", function ($scope, $http) {
    $scope.getRolesData = function () {
        $http.get("https://localhost:7179/api/Role")
            .then(function (response) {
                $scope.rolesData = response.data;
            }, function (error) {
                console.error("Failed to fetch roles:", error);
                $scope.rolesData = [];
            });
    }

    $scope.addClicked = function () {
        $scope.resetSearchMessages();
        $scope.activeSection = 'upsert';
    }

    $scope.cancelClicked = function () {
        $scope.resetUpsertForm();
        $scope.activeSection = 'search';
    }

    $scope.updateClicked = function (id, heroRole, logoUrl, primaryFunction, keyAttributes) {
        $scope.resetSearchMessages();

        $scope.roleId = id;
        $scope.heroRoleModel = heroRole;
        $scope.logoUrlModel = logoUrl;
        $scope.primaryFunctionModel = primaryFunction;
        $scope.keyAttributesModel = keyAttributes;

        $scope.activeSection = 'upsert';
    }

    $scope.resetUpsertForm = function () {
        $scope.roleId = null;
        $scope.heroRoleModel = "";
        $scope.logoUrlModel = "";
        $scope.primaryFunctionModel = "";
        $scope.keyAttributesModel = "";

        $scope.saveErrors = "";
        $scope.deleteErrorMsg = "";
    }

    $scope.saveClicked = function () {
        if ($scope.heroRoleModel === "") {
            $scope.saveErrors = "Role is required.";
            return;
        }

        $scope.getRolesData();
        const duplicateRole = $scope.rolesData.filter(role => role.heroRole.toLowerCase() === $scope.heroRoleModel.trim().toLowerCase() && role.id !== $scope.roleId);
        if (duplicateRole.length > 0) {
            $scope.saveErrors = "Role already exists.";
            return;
        }

        var roleData = {
            heroRole: $scope.heroRoleModel,
            logoUrl: $scope.logoUrlModel,
            primaryFunction: $scope.primaryFunctionModel,
            keyAttributes: $scope.keyAttributesModel
        };

        if ($scope.roleId) {
            // Update existing role
            $http.put("https://localhost:7179/api/Role/" + $scope.roleId, roleData)
                .then(function (response) {
                    $scope.successMsg = "Role updated successfully!";
                    $scope.resetUpsertForm();
                    $scope.activeSection = 'search';
                    // Refresh roles data
                    return $http.get("https://localhost:7179/api/Role");
                })
                .then(function (response) {
                    $scope.rolesData = response.data;
                })
                .catch(function (error) {
                    console.error("Failed to update role:", error);
                    alert("Error updating role.");
                });
        } else {
            // Add new role
            $http.post("https://localhost:7179/api/Role", roleData)
                .then(function (response) {
                    $scope.successMsg = "Role added successfully!";
                    $scope.resetUpsertForm();
                    $scope.activeSection = 'search';
                    // Refresh roles data
                    return $http.get("https://localhost:7179/api/Role");
                })
                .then(function (response) {
                    $scope.rolesData = response.data;
                })
                .catch(function (error) {
                    console.error("Failed to add role:", error);
                    alert("Error adding role.");
                });
        }
    }

    $scope.deleteClicked = function (id) {
        $scope.resetSearchMessages();

        $http.get("https://localhost:7179/api/Hero")
            .then(function (response) {
                const heroesData = response.data;
                const roleIsUsed = heroesData.some(hero =>
                    Array.isArray(hero.roles) && hero.roles.some(role => role.id === id)
                );

                if (roleIsUsed) {
                    $scope.deleteErrorMsg = "This role is assigned to one or more heroes. Please remove the role from the heroes before deleting it.";
                    return;
                }

                if (confirm("Are you sure you want to delete this role?")) {
                    $http.delete("https://localhost:7179/api/Role/" + id)
                        .then(function (response) {
                            $scope.successMsg = "Role deleted successfully!";
                            // Refresh roles data
                            return $http.get("https://localhost:7179/api/Role");
                        })
                        .then(function (response) {
                            $scope.rolesData = response.data;
                        })
                        .catch(function (error) {
                            console.error("Failed to delete role:", error);
                            alert("Error deleting role.");
                        });
                }
            }, function (error) {
                console.error("Failed to fetch heroes:", error);
                $scope.heroesData = [];
            });
    }

    $scope.resetSearchMessages = function () {
        this.successMsg = "";
        this.deleteErrorMsg = "";
    }

    $scope.rolesData = [];
    $scope.getRolesData();

    $scope.activeSection = "search";

    $scope.heroRoleModel = "";
    $scope.logoUrlModel = "";
    $scope.primaryFunctionModel = "";
    $scope.keyAttributesModel = "";

    $scope.roleId = null;

    $scope.saveErrors = "";
    $scope.successMsg = "";
    $scope.deleteErrorMsg = "";
});