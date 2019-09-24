# Hobbyist Framework

This is my personal project that I plan to use myself once completed.

## Todo - [Dialogue]
* [Develop Nodes]
* [Improve IntegerInput]
* [Develop Visual Node Editor]

## Planned Features
* [Items]
* [Quests]
* [Map Editor]
* [Character Controller]

# Setup
## Deployer Setup
Before proceeding with the upgrade, we need to build the new deployer.
Download the OVA template here: http://engci-maven-master.cisco.com/artifactory/spvss-ivp-deployRC/ivp-coe-deployer-201909180201-3.7.2.ova

If you are taking the old deployer down before building the new deployer, copy the following files to an external host:
```
tar -czf /etc/hosts <ivp_coe_dir>/inventory <kube_config> deployer_files.tar.gz
scp deployer_files.tar.gz <host>:
```
*Note:* The kube_config file will be named like: admin.kubeconfig-DOMAIN. Where DOMAIN relates to your environment.
*Example:* admin.kubeconfig-dldc.tx.charter.com

The new deployer must have:
*	2 vCPU’s
*	4 GB of RAM
*	80 GB of storage
*Note:* You may have to set the disk space after building the VM if you can’t increase the disk size.

After bringing up the new deployer, copy the deployer files over.
```
scp deployer_files.tar.gz <new_deployer>:
```

Login to the deployer and move the files to the necessary locations
```
mkdir deployer_files
mv deployer_files.tar.gz deployer_files/
cd deployer_files/
tar -xzvf deployer_files.tar.gz
sudo cp hosts /etc/hosts
cp inventory /home/centos/ivp-coe/inventory
```

Ensure the new deployer has the same ip address as the previous one by editing the ifcfg file.
We simply need to change the IPADDR, GATEWAY, and NETMASK/PREFIX values to mirror the values from the old deployer:
```
vi /etc/sysconfig/network-scripts/ifcfg-eth0
service network restart
```

Additionally, the new deployer must have the vmr-bundle downloaded.
Login to an external host to download the bundle:
```
mkdir vmr-bundle
cd vmr-bundle
wget -r -np -nH --cut-dirs=4 --no-parent --exclude-directories=.svn --reject "index.html*,vmr_release*" http://vmr-release-server.cisco.com/index.html/vmr-cisco-<version>/deployment/vmr-bundle/
```
*Example:* wget -r -np -nH --cut-dirs=4 --no-parent --exclude-directories=.svn --reject "index.html*,vmr_release http://vmr-release-server.cisco.com/index.html/vmr-cisco-1.5.1_022/deployment/vmr-bundle/

Then copy the vmr-bundle folder to the deployer
```
scp -pr vmr-bundle <deployer>:
```

Next, login to the openshift cluster, and focus on the vmr project.
oc login <host> -u <user> -p <pass>
oc project vmr
*Example:* oc login localhost:8443 -u system -p admin
*Note:* If you are unable to use any oc commands after logging in, you must set the KUBECONFIG variable
```
export KUBECONFIG=deployer_files/<kube_config>
```

While we are on the deployer, we should also make a backup of the cluster before we start upgrading anything
```
cd <ivp_coe_dir>
ansible-playbook -i inventory ops/cluster-backup.yml
```

If the above command fails, (does not populate the openshift-cluster-backup directory), manually backup the cluster:
```
tar -czf <backup_files> ./backup_files.tar.gz
```

Additionally, you must provide a vmr-inventory file for use during the VMR upgrade process.
Once you have configured/vetted the vmr-inventory file. Copy it to the deployer, and move it under the vmr-bundle/inventories directory.
```
scp <vmr-inventory> <deployer>:<vmr-bundle>/inventories/
```
*Note:* It is important that the vmr-inventory file is located under the vmr-bundle/inventories directory later on when we perform the vmr upgrade.

## MemSQL Setup
Before proceeding with the MemSQL upgrade, ensure that you have the following files on the memsql master aggregator:
*	memsqlbin_amd64.tar.gz
*	memsql-ops-<ops-version>.tar.gz

If these files do not exist, download them, with the following commands:
```
curl -O http://download.memsql.com/releases/version/<memsql-version>/memsqlbin_amd64.tar.gz
curl -O http://download.memsql.com/memsql-ops-<ops-version>/memsql-ops-<ops-version>.tar.gz
```
*Note:* If you are in an environment with limited network availability, the above commands will not work. Execute them on an external machine, and then copy them to the memsql master aggregator with the following commands:
```
scp memsqlbin_amd64.tar.gz <memsql-master-host>:
scp memsql-ops-<ops-version>.tar.gz <memsql-master-host>:
```

# MemSQL Upgrade
This section describes the process of upgrading MemSQL. The upgrade requires downtime of the MemSQL cluster, and should be performed during off-peak hours.

## Pre-Checks
On a memsql node, enter the memsql console, and focus on the riodev database with the commands
```
memsql
USE riodev;
```

Next, check the health of the cluster, along with its partitions, leaves, and aggregators; and that they are healthy, online, or replicating.
```
SHOW CLUSTER STATUS;
SHOW PARTITIONS EXTENDED;
SHOW LEAVES;
SHOW AGGREGATORS;
```

Finally, check the details of the RESTORE REDUNDANCY and REBALANCE PARTITIONS queries, along with the redundancy_level variable.
```
EXPLAIN RESTORE REDUNDANCY;
EXPLAIN REBALANCE PARTITIONS;
SHOW VARIABLES like "redundancy_level";
exit;
```
*Note:* The redundancy_level variable must be ‘2’, and the queries may not be set.

From the deployer, note the openshift pod counts for the vmr components. It is very important that we do this before scaling deployments down.
```
oc get deploy
```

Before performing the upgrade, check for mounted NFS volumes on the memsql node:
```
mount | grep backup
```
*Note:* The mount directory might not have the word backup in its name. This is simply an example and the name will vary. From here on we will refer to the mounted directory as /mnt/backup_nfs

If no mount exists for the database backup, execute the following commands to create the directory and perform the mount on the memsql node.
```
mkdir /mnt/backup_nfs
mount <nfs_volume> /mnt/backup_nfs
```
*Note:* The nfs_volume refers to a shared volume managed by your virtualization tool (vSphere, VMWare, etc.)

## Upgrade
From the deployer, perform the following command to scale down vmr components.
```
oc scale deploy a8-updater --replicas=0
oc scale deploy dash-origin --replicas=0
oc scale deploy archive-agent --replicas=0
oc scale deploy manifest-agent --replicas=0
oc scale deploy segment-recorder --replicas=0
oc scale deploy recorder-manager --replicas=0
```
*Note:* It is important that we have taken note of the pod counts so we can restore the deployment configurations later.

From the memsql node, backup the database to the directory where the NFS volume is mounted:
```
memsql
BACKUP DATABASE riodev TO "/mnt/backup_nfs";
exit
```

After scaling the deployments and backing up the database, upgrade memsql-ops on the cluster nodes.
```
memsql-ops agent-upgrade --file-path=<memsql-ops.tar>
```

Verify the memsql-ops version on all cluster nodes:
```
memsql-ops agent-list
```

Afterwards, stop all of the memsql nodes in the cluster:
```
memsql-ops memsql-stop --all
```

**If a node does stop**
Run the following commands to refresh its state.
```
memsql-ops memsql-unmonitor <id>
memsql-ops memsql-monitor -h <host> -u <user> -p <pass>
```
Once you confirm that all nodes have stopped (memsql-ops memsql-list) proceed.

After stopping the memsql process on all of the cluster nodes, proceed to upgrade memsql:
```
memsql-ops memsql-upgrade --skip-snapshot --skip-version-check --no-backup-data-directories --file-path=<memsqlbin.tar>
```

Then verify the memsql version with the following command:
```
memsql-ops memsql-list
```

From the deployer, restore the vmr pod counts.
```
oc scale deploy a8-updater --replicas=<count>
oc scale deploy dash-origin --replicas=<count>
oc scale deploy archive-agent --replicas=<count>
oc scale deploy manifest-agent --replicas=<count>
oc scale deploy segment-recorder --replicas=<count>
oc scale deploy recorder-manager --replicas=<count>
```
*Note:* The pod counts relate to the notes taken from the start of the upgrade process.

## Verifications
After performing the upgrade, verify that the cluster, aggregators, and leaves are healthy, online, and replicating.
```
memsql
SHOW LEAVES;
SHOW AGGREGATORS;
SHOW CLUSTER STATUS;
exit;
```

We can also monitor memsql, and check it's version through the memsql-ops ui:
```
http:<memsql-master>:9000
```
*Example:* http://24.165.224.202:9000

# CentOS Upgrade
## Pre-Checks
From the deployer, check the status of the vmr components with the commands:
```
oc get pods --all-namespaces -o wide | tee pods.status
oc get nodes -o wide | tee nodes.status
```
*Note:* We tee the status of the pods and nodes for comparison later

## Upgrade
Before performing the upgrade, set the ANSIBLE_LOG_PATH before upgrading:
```
export ANSIBLE_LOG_PATH=./upgrade_os.$(date +%Y_%m_%d_%H_%M)
```

To perform the upgrade run the following commands from the deployer as centos. The load_registry playbook takes ~10 minutes to finish, while the upgrade_os playbook takes approximately 7-15 minutes per node.
```
cd /home/centos/ivp-coe/
ansible-playbook -i inventory load_registry.yml
ansible-playbook -i inventory ops/upgrade_os.yml
```

## *[Notes]*
A node may be stuck in the SchedulingDisabled state if the upgrade process was interrupted, causing the playbook to fail on future attempts. You can fix this with the following commands.
```
oc get nodes
oc adm manage-node <node_name> --shedulable=true
```

If the process complains that pods are in the Error state, you must investigate the health of all pods. If the pods are healthy and the error persists, run the following command to continue the upgrade.
```
ansible-playbook -i inventory ops/upgrade_os.yml -e "wait_pod_start=90" -e "wait_after_reboot=120"
```
*Note:* The values may vary according to your situation.

If the upgrade complains that an origin service is not running, login to the node and check the status of the service.
```
sudo systemctl status <origin-service>
```

If offline, attempt to restart it.
```
sudo systemctl start <origin-service>
```

Once you get the origin service running, restart the playbook to continue the upgrade.
```
ansible-playbook -i inventory ops/upgrade_os.yml
```

## Verification
Verify the CSCOlxplat rpm is installed, and that the nodes have restarted.
```
ansible OSEv3 -i inventory -m shell -a "rpm -q CSCOlxplat-CentOS"
ansible OSEv3 -i inventory -m shell -a "uptime"
```

Additionally, check that the pods and nodes are Running. Compare the output with the status files we created in the Pre-Checks step.
```
oc get nodes -o wide
oc get pods --all-namespaces -o wide
```
*Note:* <invalid> ages should resolve themselves over time.

# COE Upgrade
## Upgrade
To perform the upgrade, run the following commands on the deployer as **centos**.
```
cd /home/centos/ivp-coe/
./run_installation install inventory
```
*Note:* It is important that the command is ran as the centos user, and not root. This process takes approximately 15-40 minutes.

## Verification
Check the cluster status with the commands to ensure everything is running and healthy:
```
oc status
oc get nodes
oc get all
```
*Note:* <invalid> ages should resolve themselves over time.

# VMR Upgrade
## Setup
From the deployer, load the docker images from the vmr-bundle:
```
cd /home/centos/vmr-bundle/
sudo docker load -i <vmr-bundle.tar.gz>
cp manifest.json rio-docker-images/
sudo su
docker-compose run vmr-bundle ./load.sh <images_dir> <manifest_json> <docker_registry_host>:5000
```
*Example:* docker-compose run vmr-bundle ./load.sh rio-docker-images/ rio-docker-images/manifest.json 172.27.11.75:5000
*Note:* If you receive an error saying the docker daemon is not running, check that the docker service is running, or run the command as root.

After loading the vmr-bundle images, verify the schema-upgrader image exists, and tag it.
```
docker images | grep schema-upgrader
docker tag <schema-upgrader-image:version> schema-upgrader
```
*Example:* docker tag dockerhub.cisco.com/vmr-docker/vmr_release/schema-upgrader:cisco-1.5.1_022 schema-upgrader
*Note:* You can see that the docker image was tagged by running. You should see an additional image this time simply named schema-upgrader
```
docker images | grep schema-upgrader
```

## Schema Upgrade
After loading the vmr-bundle and tagging the schema-upgrader, perform the schema-upgrade.
```
docker run --rm --net=host schema-upgrader --host=<memsql-master-ip>
```

## Upgrade
After upgrading the memsql schema, upgrade VMR.
```
docker-compose run vmr-bundle ./deploy.sh inventories/<inventory>
```
*Note:* It is important that the vmr-inventory file is inside the inventories directory, or else the compose command may not run.

## Verification
We might have to re-login to openshift once the vmr upgrade is complete.
```
oc login
oc project
```

Afterwards, ensure that the deployments and pods are all healthy, running, or replicating. If there are any errors, you must go through and resolve them.
```
oc get deploy
oc get pods
```

Once finished, log out of openshift.
```
oc logout
```

Finally, test playback to ensure that VMR is functioning properly.
