angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter, folder) {

    $scope.close = () => $mdDialog.hide();
    $scope.folder = folder;

    $scope.test = () => {
        console.log("-->", $scope.folder);
        $http.get('/api/Photo/GetFolders').then(x => {
            $scope.photos = x.data;
            $scope.num = $scope.photos.length;

            //photo filter by folder or code?
            console.log($scope.photos);
            
        });
    }

   
    $scope.tags = ['pretty', 'travel', 'dog'];

    $scope.updateTags = () => {
        //angular.forEach(values, function (value, key) {
        //    $scope.photos.Tags.push(tags);
        //});

        //$http.put('/api/Photo/' + folder, photos).then(res => console.log(res));
    }

    

});