const test = {
    url: process.env["api_url"],
    debug: process.env["debug"],

    headers: {
        "Accept": "*/*",
        "Content-Type": "application/json"
    },
    
    users: [
        {
            document: `22345676${generateNum(999, 10)}`,
            password: `${generateNum(100000), 100000}`,
            email: `vainafe+${generateNum(100, 1)}@goo${generateNum(10000, 1)}.com`,
            id: 0
        }
    ],

    organizations: [
        {
            document: `10799843000${generateNum(980, 10)}`,
            email: `vainafe+${generateNum(100, 1)}@goo${generateNum(10000, 1)}.com`,
            id: 0
        }
    ],
    
    routines: [
        {
            name: "CreateUser",
            fn: function(resolve, reject) {
                const user = test.users[0];

                return fetch(test.url + "/user", {
                    method: "POST",
                    headers: test.headers,
                    body: JSON.stringify({
                        document: user.document,
                        email: user.email,
                        name: "Annelise Keating",
                        password: user.password
                    })
                }).then(async (res) => {

                    if (res.status != 201 || getLocationHeader(res.headers).length == 0){
                        reject({
                            err: "user creation was not succesful or location header was not found",
                            res: res,
                        })
                    }
    
                    test.users[0].id = getLocationHeader(res.headers);
                    resolve()
                })
            }
        },
        {
            name: "AuthenticateUser",
            fn: function(resolve, reject) {
                const user = test.users[0];

                return fetch(test.url + "/auth", {
                    method: "PUT",
                    headers: test.headers,
                    body: JSON.stringify({
                        email: user.email,
                        password: user.password
                    })
                }).then(async (res) => {
                    if (res.status != 204){
                        reject({
                            err: "authentication was not succesful",
                            res: res,
                        })
                    }
    
                    test.headers.authorization = "Bearer " + res.headers.get("authorization");
                    resolve();
                })
            }
        },
        {
            name: "GetUserById",
            fn: function(resolve, reject) {
                const userId = test.users[0].id;

                return fetch(test.url + "/user/" + userId.toString(), {
                    method: "GET",
                    headers: test.headers,
                }).then(async (res) => {
                    if (res.status != 200){
                        reject({
                            err: "user retrieval was not succesful",
                            res: res,
                        })
                    }

                    resolve(res.json());
                })
            }
        },

        {
            name: "CreateOrganization",
            fn: function(resolve, reject) {
                const org = test.organizations[0];

                return fetch(test.url + "/organizations", {
                    method: "POST",
                    headers: test.headers,
                    body: JSON.stringify({
                        document: org.document,
                        phoneNumber: "4322222222",
                        name: "Annelise Keating ONG's " + generateNum(1000, 1),
                        coordinateX: 10.77,
                        coordinateY: 103.77,
                        addressName: "Street Boys, " + generateNum(1000, 1),
                    })
                }).then(async (res) => {

                    if (res.status != 201 || getLocationHeader(res.headers).length == 0){
                        reject({
                            err: "org creation was not succesful or location header was not found",
                            res: res,
                        })
                    }
    
                    test.organizations[0].id = getLocationHeader(res.headers);
                    resolve()
                })
            }
        },

        {
            name: "GetOrganizationById",
            fn: function(resolve, reject) {
                const org = test.organizations[0];

                return fetch(test.url + "/organizations/" + org.id.toString(), {
                    method: "GET",
                    headers: test.headers,
                }).then(async (res) => {

                    if (res.status != 200){
                        reject({
                            err: "org retrieval was not succesful",
                            res: res,
                        })
                    }

                    resolve(res.json())
                })
            }
        },

        {
            name: "ListOrganizationMembers",
            fn: async function(resolve, reject) {
                const org = test.organizations[0];

                var page = "start=0&end=9"
                var hasNext = true;
                var items = [];

                while (hasNext) {
                    await fetch(test.url + `/organizations/${org.id.toString()}/members?` + page, {
                        method: "GET",
                        headers: test.headers,
                    }).then(async (res) => {
    
                        if (res.status != 200){
                            reject({
                                err: "org retrieval was not succesful",
                                res: res,
                            })
                        }

                        return res.json()
                    }).then(({data}) => {
                        hasNext = data?.hasNext
                        page = data?.nextPage
                        items = [
                            ...items,
                            ...data?.items
                        ]

                    })
                }

                resolve(items)
            }
        },
    ]
};

(async() => {
    console.info("Started tests")

    for (const routine of test.routines) {
        console.info(`Running routine: ${routine.name}`);

        try {
            var res = await new Promise(routine.fn)
            if (test.debug){
                console.debug(res);
            }

        } catch (error) {
            if (error.err && error.res){
                error = {
                    err: `${routine.name}: ${error.err}`,
                    details: await debugResponse(error.res)
                }
            } 
            
            console.error(error);

            return
        }

        console.info(`Finished routine successfully: ${routine.name}`);
    }
    
    console.info("Finished tests successfully")
    
})();




function generateNum(until, skip) {
    return Math.floor(Math.random() * until) + skip
}

function getLocationHeader(h) {
    return h.get("location")?.toString() ?? ""
}

async function debugResponse(res = new Response()){
    var headers = {};
    res.headers.forEach((v, k) => {
        headers[k] = v
    })
    
    return {
        headers: headers,
        status: res.status,
        text: await res.text(),
        url: res.url
    }
}