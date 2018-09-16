app.controller('RandomPhotoCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    //let area = document.getElementsByClassName("main-landing")[0];
    $scope.getRandomPosition = (element) => {
        var x = 800 - element.clientHeight;
        var y = document.body.offsetWidth - element.clientWidth;
        //var randomX = Math.floor(Math.random() * x);
        //var randomY = Math.floor(Math.random() * y);
        let coords = [getRandomInt(0, x), getRandomInt(650, 900)];
        return coords;
    }

    //$scope.loadImages = (ph) => {
    //    var img = document.createElement('img');
    //    img.setAttribute("src", ph.Url);
    //    img.setAttribute("style", "position:absolute; border-radius: 20px; max-width: 400px; max-height: 400px; margin: 50px;");
    //    var xy = $scope.getRandomPosition(img);
    //    img.style.top = xy[0] + 'px';
    //    img.style.left = xy[1] + 'px';

    //    var element = document.getElementById("main-landing");
    //    element.appendChild(img);
    //};

    $scope.photoList = [];

    $scope.getRandomPhoto = () => {
        $http.get('/api/Photo/GetPublicIds').then(x => {
            $scope.randomize(x.data);
        });
    }; 

    $scope.randomize = (photos) => {        
        let counter = 0;
        let interval = setInterval(function () {
            counter += 1;
            if (counter === 15) {
                clearInterval(interval);
            }
            let idList = photos;

            var id = idList[getRandomInt(0, photos.length - 1)];
            $http.get('/api/Photo/' + id).then(x => {
                if ($scope.photoList['Id'] != id) {
                    $scope.photoList.push(x.data);
                }
            });

        }, 0);
    };

}]);