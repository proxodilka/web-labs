import subprocess as sub
import multiprocessing
import pandas
import argparse
import pandas

args = {}


def interface():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--executor",
        "-e",
        required=False,
        default="ServerBenchmark.exe",
        help="Path to server benchmark.",
    )
    parser.add_argument(
        "--num_clients",
        "-n",
        required=False,
        default=50,
        help="Number of client instances.",
    )
    parser.add_argument(
        "--server_ip", "-i", required=False, default="127.0.0.1", help="Server ip."
    )
    parser.add_argument(
        "--server_port", "-p", required=False, default="8080", help="Server port."
    )

    return vars(parser.parse_args())


# res = []


def run_solver(args, i, res):
    proc = sub.Popen(
        [args["executor"], args["server_ip"], args["server_port"]],
        stdin=sub.PIPE,
        stdout=sub.PIPE,
        stderr=sub.PIPE,
        text=True,
    )
    outs, errs = proc.communicate()
    # pandas.Series(map(int, outs.splitlines()))
    # res.append(outs)
    res[f"client{i}"] = outs
    # return outs


if __name__ == "__main__":
    args.update(interface())

    res = {}

    manager = multiprocessing.Manager()
    res = manager.dict()

    processes = []
    for i in range(int(args["num_clients"])):
        p = multiprocessing.Process(target=run_solver, args=[args, i, res])
        processes.append(p)

    [p.start() for p in processes]

    for p in processes:
        p.join()

    [p.kill() for p in processes]

    df = pandas.concat(
        [pandas.Series(map(int, v.splitlines()), name=k) for k, v in res.items()], axis=1
    )
    df.to_csv("result.csv")
