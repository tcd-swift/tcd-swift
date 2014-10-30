DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
docker run -v "$DIR:/opt/tcd-swift" -t -i ianconnolly/tcd-swift /bin/bash
