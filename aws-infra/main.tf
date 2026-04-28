module "vpc" {
  source = "./modules/vpc"

  name_prefix       = var.name_prefix
  vpc_cidr          = var.vpc_cidr
  existing_vpc_id   = var.existing_vpc_id
}

module "subnet" {
  source = "./modules/subnet"

  name_prefix      = var.name_prefix
  vpc_id           = module.vpc.vpc_id
  subnet_cidr      = var.subnet_cidr
  availability_zone = var.availability_zone
}

module "igw" {
  source = "./modules/igw"

  name_prefix = var.name_prefix
  vpc_id      = module.vpc.vpc_id
  subnet_id   = module.subnet.subnet_id
}

module "ec2" {
  source = "./modules/ec2"

  name_prefix       = var.name_prefix
  vpc_id            = module.vpc.vpc_id
  subnet_id         = module.subnet.subnet_id
  instance_type     = var.instance_type
  allowed_ssh_cidr  = var.allowed_ssh_cidr
  app_port          = var.app_port
  app_type          = var.app_type
}