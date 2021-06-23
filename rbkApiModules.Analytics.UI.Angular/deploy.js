const FtpDeploy = require("ftp-deploy");
const ftpDeploy = new FtpDeploy();

const enviroment = process.argv[2];
let envPath = "";

if (enviroment == null) {
    console.log("No argument specified");
    return;
}
else if (enviroment.toLowerCase().includes("dev")){
    envPath = "development";
} else if (enviroment.toLowerCase().includes("prod")){
    envPath = "production";
} else {
    console.log("Invalid Argument");
    return;
}

const config = {
    user: "medical",
    // Password optional, prompted if none given
    password: "",
    host: "ftp.horizon-solutions.tk",
    port: 21019,
    localRoot: __dirname + "/dist",
    remoteRoot: `/${envPath}/wwwroot/professional/`,
    // include: ["*", "**/*"],      // this would upload everything except dot files
    include: ["*", "**/*"],
    // e.g. exclude sourcemaps, and ALL files in node_modules (including dot files)
    exclude: [""],
    // delete ALL existing files at destination before uploading, if true
    deleteRemote: true,
    // Passive mode is forced (EPSV command is not sent)
    forcePasv: true,
    // use sftp or ftp
    sftp: false,
    secure: true,
    keepalive: 5000,
    secureOptions: {
        rejectUnauthorized: false
    }
};

ftpDeploy
    .on("log", function(data) {
        if (!data.includes("Files found to upload:")){
            console.log(`\n${data}`);
        }
    })
    .on("uploading", function (data) {
        const percentage = ((100 * data.transferredFileCount) / data.totalFilesCount).toFixed();
        const factor = (percentage / 5).toFixed();

        const dots = ".".repeat(factor);
        const empty = " ".repeat(20 - factor);
        const nameEmpty = " ".repeat(100 - data.filename.length);

        /* need to use  `process.stdout.write` becuase console.log print a newline character */
        /* \r clear the current line and then print the other characters making it looks like it refresh*/
        process.stdout.write(`\rUploading [${dots}${empty}] ${percentage}% : ${data.filename}${nameEmpty}`);
    })
    .on("upload-error", function (data) {
        console.log(`\n${data.err}`); // data will also include filename, relativePath, and other goodies
    })
    .deploy(config)
    .then(_ => console.log(`\nFinished!`))
    .catch(err => console.log(`\n${err}`));