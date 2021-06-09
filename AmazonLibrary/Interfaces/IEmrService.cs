namespace AmazonLibrary.Interfaces {

    public interface IEmrService {

        bool ExecuteCommandMapper();

        bool ExecuteCommandReducer();
    }
}