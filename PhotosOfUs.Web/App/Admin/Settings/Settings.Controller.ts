
namespace PhotosOfUs {
    angular.module('app').controller('SettingsCtrl', ['$scope', 'Upload', 'EventApiClient', '$window',
     ($scope, Upload, EventApiClient: EventApiClient, $window) => {
        $scope.settings = <IEvent>{
            eventId: 0,
            createdDate: new Date(),
            overlayOpacity: 0,
            separatorThickness: 0,
            separatorWidth: 0,
            homepageTemplate: '2',
            headerColorCode: '#000000',
            accentColorCode: '#ff6060',
            backgroundColorCode: '#f6ffff',
            bodyColorCode: '#000000',
            overlayColorCode: '#000000',
            brandingStyle: 1,
            separatorStyle: 'Solid'
        };

        $scope.colorPickerOptions = {
            format: 'hex8String',
            case: 'lower'
        };

        $scope.$watch('settings.featuredImage', () => {
            if ($scope.settings.featuredImage && !$scope.settings.featuredImage.error) {
                Upload.upload({
                    url: '/api/Event/Photos',
                    data: {
                      file: $scope.settings.featuredImage
                    }
                }).then(resp => {
                    $scope.settings.featuredImageUrl = resp.data.url;
                }, null, evt => {
                    const progressPercentage = 100.0 * evt.loaded / evt.total;
                    console.log('progress: ' + progressPercentage +
                      '% ' + evt.config.data.file.name);
                });
            }
        });

        $scope.removeFeaturedImage = ($event) => {
            $scope.settings.featuredImageUrl = null;
            $scope.settings.featuredImage = null;
            $event.stopPropagation();
        };

        $scope.save = () => {
            EventApiClient.save($scope.settings).then(x => $window.location.href = '/Admin/Cards/' + x.eventId);
        };
    }]);
}
