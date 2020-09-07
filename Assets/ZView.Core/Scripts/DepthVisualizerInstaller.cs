using UnityEngine;
using Zenject;

namespace ZView
{
    [RequireComponent(typeof(DepthVisualizer))]
    public class DepthVisualizerInstaller : MonoInstaller
    {
        [SerializeField] FirebasePointCloudDatabase firebaseMeshDatabase;
        [SerializeField] MeshDataListPanelView dataListView;

        public override void InstallBindings()
        {
            var depthVisualizer = GetComponent<DepthVisualizer>();

            Container.BindInterfacesTo<DepthVisualizer>()
                .FromInstance(depthVisualizer);

            Container.Bind<IPointCloudDatabase>()
                .FromInstance(firebaseMeshDatabase);

            Container.BindInterfacesAndSelfTo<DepthVisualizerUIPresenter>()
                .AsSingle();

            Container.Bind<IMeshDataListUIView>()
                .FromInstance(dataListView);
        }
    }
}