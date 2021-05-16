using System.Collections.Generic;

namespace Denhub.API.Results {
    public class SuccessResult : Result {
        public override ResultType Type => ResultType.Ok;
        public override List<string> Errors => new();
    }
}